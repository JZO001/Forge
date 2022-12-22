/* *********************************************************************
 * Date: 9 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Forge.Logging.Abstraction;
using Forge.Management;
using Forge.Native;
using Forge.Native.Helpers;
using Forge.Native.Hooks;
using Forge.Net.Remoting.Channels;
using Forge.Net.Remoting.Service;
using Forge.Net.Services;
using Forge.Net.Services.Services;
using Forge.RemoteDesktop.Contracts;
using Forge.RemoteDesktop.Service.Configuration;
using Forge.Shared;

namespace Forge.RemoteDesktop.Service
{

    internal delegate void SetClipboardContentHandler(string text);
    internal delegate Cursor GetCurrentCursorHandler();
    internal delegate void SaveFileHandler(string fileName, Stream stream);

    /// <summary>
    /// Remote Desktop Service Manager
    /// </summary>
    public sealed class RemoteDesktopServiceManager : RemoteServiceBase<IRemoteDesktop, RemoteDesktopServiceImpl, RemoteDesktopServiceManager>
    {

        #region Field(s)

        private readonly Dictionary<string, IRemoteDesktopInternalClient> mClients = new Dictionary<string, IRemoteDesktopInternalClient>();

        private readonly Dictionary<string, TimeoutWatch> mClientsWatchForTimeout = new Dictionary<string, TimeoutWatch>();

        private readonly Dictionary<string, IRemoteDesktopInternalClient> mClientsAccepteds = new Dictionary<string, IRemoteDesktopInternalClient>();

        private readonly List<ThreadClientContext> mWorkerThreadContainers = new List<ThreadClientContext>();

        private readonly Dictionary<IRemoteDesktopInternalClient, ThreadClientContext> mClientVsThreadClients = new Dictionary<IRemoteDesktopInternalClient, ThreadClientContext>();

        private readonly object mLockObjectForClients = new object();

        private static readonly Dictionary<Cursor, string> mCursorWithIds = new Dictionary<Cursor, string>();

        private Thread mTimeoutWatchThread = null;

        private readonly AutoResetEvent mTimeoutWatchEvent = new AutoResetEvent(false);

        private bool mConfigAcceptKeyboardAndMouseInputFromClients = false;

        private int mConfigDesktopImageClipHeight = Consts.DEFAULT_DESKTOP_IMAGE_CLIP_SIZE;

        private int mConfigDesktopImageClipWidth = Consts.DEFAULT_DESKTOP_IMAGE_CLIP_SIZE;

        private DesktopShareModeEnum mConfigDesktopShareMode = DesktopShareModeEnum.Shared;

        private Thread mScreenCaptureThread = null;

        private int mScreenCapturePollInterval = 1000; // TODO: ezt konfigba kitenni

        private readonly AutoResetEvent mScreenCaptureIdleEvent = new AutoResetEvent(false);

        private ScreenDataContainer mScreenDataContainer = null;

        private readonly ManualResetEvent mScreenClipsInitialized = new ManualResetEvent(false);

        private static ImageCodecInfo mImageCodecInfoJpg = null;

        private readonly Forge.Threading.ThreadPool mClipSenderToClientsThreadPool = new Threading.ThreadPool("RemoteDesktop_ClipSenderToClients");

        private Thread mMouseMoveSenderThread = null;

        private readonly AutoResetEvent mMouseMoveEvent = new AutoResetEvent(false);

        private Point mMouseMovePosition = Point.Empty;

#if NET40
        private static System.Security.Cryptography.SHA1Managed mCrcCalculator = new System.Security.Cryptography.SHA1Managed();
#else
        private static System.Security.Cryptography.SHA1 mCrcCalculator = System.Security.Cryptography.SHA1.Create();
#endif

        private static UIAccessorControl mUIAccessor = null;

        /// <summary>
        /// Occurs when [event connection state change].
        /// </summary>
        public event EventHandler<ConnectionStateChangedEventArgs> EventConnectionStateChange;

        /// <summary>
        /// Occurs when [event accept client].
        /// </summary>
        public event EventHandler<AcceptClientEventArgs> EventAcceptClient;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDesktopServiceManager"/> class.
        /// </summary>
        public RemoteDesktopServiceManager()
            : base(Consts.SERVICE_ID)
        {
            ApplyConfiguration();
            Settings.EventConfigurationChanged += new EventHandler<EventArgs>(Settings_EventConfigurationChanged);
            mTimeoutWatchThread = new Thread(new ThreadStart(TimeoutWatchThreadMain));
            mTimeoutWatchThread.Name = "RemoteDesktopTimeoutWatcher";
            mTimeoutWatchThread.IsBackground = true;
            mTimeoutWatchThread.Start();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the connected clients.
        /// </summary>
        /// <value>
        /// The connected clients.
        /// </value>
        public List<IRemoteDesktopService> ConnectedClients
        {
            get
            {
                lock (mLockObjectForClients)
                {
                    return mClients.Values.ToList<IRemoteDesktopService>();
                }
            }
        }

        /// <summary>
        /// Gets the active clients.
        /// </summary>
        /// <value>
        /// The active clients.
        /// </value>
        public List<IRemoteDesktopService> ActiveClients
        {
            get
            {
                lock (mLockObjectForClients)
                {
                    return mClientsAccepteds.Values.ToList<IRemoteDesktopService>();
                }
            }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Starts the service.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="serviceDescriptor">The service descriptor.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override ManagerStateEnum Start(long priority, IServiceDescriptor serviceDescriptor)
        {
            ManagerStateEnum state = ManagerState;
            if (state != ManagerStateEnum.Started)
            {
                Cursor c = Cursor.Current; // ez csak azért kell, hogy az alkalmazás feliratkozzon az eseményekre
                Forge.Net.TerraGraf.NetworkManager.Instance.Start();
                ServiceBaseServices.Initialize();
                MouseEventHookManager.Instance.Start();
                ClipboardEventManager.Instance.Start();

                state = base.Start(priority, serviceDescriptor);
                if (state == ManagerStateEnum.Started)
                {
                    if (mUIAccessor == null)
                    {
                        mUIAccessor = new UIAccessorControl(LOGGER);
                        mUIAccessor.CreateControl();
                    }

                    mScreenCaptureThread = new Thread(new ThreadStart(ScreenCaptureThreadMain));
                    mScreenCaptureThread.Name = "RemoteDesktopService_ScreenCapture";
                    mScreenCaptureThread.IsBackground = true;
                    mScreenCaptureThread.Start();

                    mMouseMoveSenderThread = new Thread(new ThreadStart(MouseMoveSenderThreadMain));
                    mMouseMoveSenderThread.Name = "RemoteDesktopService_MouseMoveSender";
                    mMouseMoveSenderThread.IsBackground = true;
                    mMouseMoveSenderThread.Start();
                }
            }
            return state;
        }

        /// <summary>
        /// Stops the service
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override ManagerStateEnum Stop()
        {
            if (ManagerState != ManagerStateEnum.Stopped)
            {
                base.Stop();
                lock (mLockObjectForClients)
                {
                    // kihajítom a klienseket
                    mClients.Values.ToList<IRemoteDesktopService>().ForEach(i => ServiceContract_Disconnected(i, new DisconnectEventArgs(i.SessionId)));
                }
                mScreenCaptureIdleEvent.Set();
                mMouseMoveEvent.Set();
            }

            return ManagerState;
        }

        /// <summary>
        /// Disconnects the specified client.
        /// </summary>
        /// <param name="client">The client.</param>
        public void Disconnect(IRemoteDesktopPeer client)
        {
            if (client == null)
            {
                ThrowHelper.ThrowArgumentNullException("client");
            }

            ServiceContract_Disconnected(client, new DisconnectEventArgs(client.SessionId));
        }

        #endregion

        #region Internal method(s)

        internal void RegisterNewContract(IRemoteDesktopInternalClient serviceContract)
        {
            lock (mLockObjectForClients)
            {
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, registering new service contract."));
                serviceContract.Disconnected += new EventHandler<DisconnectEventArgs>(ServiceContract_Disconnected);
                ISessionInfo sessionInfo = serviceContract.Channel.GetSessionInfo(serviceContract.SessionId);
                if (sessionInfo != null)
                {
                    mClients[serviceContract.SessionId] = serviceContract;
                    mClientsWatchForTimeout[serviceContract.SessionId] = new TimeoutWatch(serviceContract);
                    mTimeoutWatchEvent.Set();
                    RaiseEvent(EventConnectionStateChange, this, new ConnectionStateChangedEventArgs(serviceContract, true));
                }
                else
                {
                    serviceContract.Disconnected -= new EventHandler<DisconnectEventArgs>(ServiceContract_Disconnected);
                }
            }
        }

        internal bool AcceptUser(IRemoteDesktopInternalClient client)
        {
            bool result = false;

            // itt regisztrálom be a usert, mint aki használhatja a szolgáltatást.
            // a szolgáltatás tényleges használatára akkor kerül sor, ha hívja a message pumpot

            lock (mLockObjectForClients)
            {
                if (ManagerState == ManagerStateEnum.Started && mClientsWatchForTimeout.ContainsKey(client.SessionId) && client.IsConnected && client.IsAuthenticated)
                {
                    mClientsWatchForTimeout.Remove(client.SessionId);

                    if (mClientsAccepteds.Count == 0 || mConfigDesktopShareMode == DesktopShareModeEnum.Shared)
                    {
                        mClientsAccepteds[client.SessionId] = client;
                        client.IsAccepted = true;
                        result = true;
                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, accepting new client. SessionId: {0}", client.SessionId));
                        RaiseEvent(EventAcceptClient, this, new AcceptClientEventArgs(client));
                    }
                    else if (mConfigDesktopShareMode == DesktopShareModeEnum.ExclusiveForLastLogin)
                    {
                        mClientsAccepteds.Values.ToList<IRemoteDesktopService>().ForEach(i => ServiceContract_Disconnected(i, new DisconnectEventArgs(i.SessionId)));
                        mClientsAccepteds[client.SessionId] = client;
                        client.IsAccepted = true;
                        result = true;
                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, accepting new client. SessionId: {0}", client.SessionId));
                        RaiseEvent(EventAcceptClient, this, new AcceptClientEventArgs(client));
                    }
                    else
                    {
                        if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, cannot accept client. SessionId: {0}", client.SessionId));
                    }
                }
                else
                {
                    // rossz sorrendben hív a kliens
                    ServiceContract_Disconnected(client, new DisconnectEventArgs(client.SessionId));
                }
            }

            return result;
        }

        internal DescriptionResponseArgs GetServiceConfiguration(IRemoteDesktopInternalClient client)
        {
            DescriptionResponseArgs result = null;

            if (ManagerState == ManagerStateEnum.Started && client.IsAuthenticated && client.IsAccepted)
            {
                result = new DescriptionResponseArgs(GetDesktopSize(), new Size(mConfigDesktopImageClipWidth, mConfigDesktopImageClipHeight), GetCursors(), mConfigAcceptKeyboardAndMouseInputFromClients);
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, sending service configuration to the client. SessionId: {0}", client.SessionId));
            }
            else
            {
                // rossz sorrendben hív a kliens
                ServiceContract_Disconnected(client, new DisconnectEventArgs(client.SessionId));
            }

            return result;
        }

        internal void SubscribeForDesktop(IRemoteDesktopInternalClient client, Area area)
        {
            bool allowToExecute = client.IsAuthenticated && ManagerState == ManagerStateEnum.Started;

            if (allowToExecute && mScreenClipsInitialized.WaitOne(5000))
            {
                // megvárjuk, amíg a screen lopó szál inicializálja a clip-eket
                lock (mLockObjectForClients)
                {
                    for (int y = 0; y < mScreenDataContainer.DesktopClips.GetLength(1); y++)
                    {
                        for (int x = 0; x < mScreenDataContainer.DesktopClips.GetLength(0); x++)
                        {
                            DesktopClip clip = mScreenDataContainer.DesktopClips[x, y];
                            if (clip.IsInArea(area))
                            {
                                // bele esik
                                if (!clip.SubscribedClients.ContainsKey(client))
                                {
                                    // feiratkozás
                                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, client subscribing for desktop clip. Location: {0}, {1} SessionId: {2}", clip.Location.X.ToString(), clip.Location.Y.ToString(), client.SessionId));
                                    ThreadClientContext threadContext = GetThreadClientContext(client);
                                    if (client.ImageQuality == Settings.DefaultImageClipQuality)
                                    {
                                        ClientContext clientContext = new ClientContext(client, clip.LastContent, threadContext.WorkEvent);
                                        clientContext.IsChanged = true;
                                        clientContext.IsRequestToResend = true;
                                        clip.SubscribedClients.Add(client, clientContext);
                                    }
                                    else
                                    {
                                        using (MemoryStream ms = new MemoryStream())
                                        {
                                            SaveImage(ms, clip.MaxQuality, client.ImageQuality);
                                            ClientContext clientContext = new ClientContext(client, ms.ToArray(), threadContext.WorkEvent);
                                            clientContext.IsChanged = true;
                                            clientContext.IsRequestToResend = true;
                                            clip.SubscribedClients.Add(client, clientContext);
                                            ms.SetLength(0);
                                        }
                                    }
                                    client.SubscribedClips.Add(clip);
                                }
                            }
                            else
                            {
                                if (clip.SubscribedClients.ContainsKey(client))
                                {
                                    // leiratkozás
                                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, client unsubscribing from desktop clip. Location: {0}, {1} SessionId: {2}", clip.Location.X.ToString(), clip.Location.Y.ToString(), client.SessionId));
                                    clip.SubscribedClients.Remove(client);
                                    client.SubscribedClips.Remove(clip);
                                }
                            }
                        }
                    }
                    if (client.IsActive)
                    {
                        GetThreadClientContext(client).WorkEvent.Set(); // mehet a küldés
                    }
                }
            }
            else
            {
                // rossz sorrendben hív a kliens
                ServiceContract_Disconnected(client, new DisconnectEventArgs(client.SessionId));
            }
        }

        internal MouseMoveServiceEventArgs StartEventPump(IRemoteDesktopInternalClient client)
        {
            MouseMoveServiceEventArgs result = null;

            bool allowToExecute = client.IsAuthenticated && ManagerState == ManagerStateEnum.Started;

            if (allowToExecute && mScreenClipsInitialized.WaitOne(5000))
            {
                lock (mLockObjectForClients)
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, starting event pump for a client. SessionId: {0}", client.SessionId));
                    result = new MouseMoveServiceEventArgs(new Point(Cursor.Position.X + mScreenDataContainer.MinValues.X, Cursor.Position.Y + mScreenDataContainer.MinValues.Y), mCursorWithIds[Cursor.Current]);
                    ((RemoteDesktopAbstractServiceProxy)client).IsActive = true;
                    GetThreadClientContext(client).WorkEvent.Set(); // allokálok egy munkavégző szálat a klienshez, ha még nem lenne...
                    mScreenCaptureIdleEvent.Set(); // ezzel kezdi a háttérszál leszedni a screen adatokat, ha eddig alukált
                }
            }
            else
            {
                // rossz sorrendben hív a kliens
                ServiceContract_Disconnected(client, new DisconnectEventArgs(client.SessionId));
            }

            return result;
        }

        internal void StopEventPump(IRemoteDesktopService client)
        {
            bool allowToExecute = client.IsAuthenticated && ManagerState == ManagerStateEnum.Started;

            if (allowToExecute)
            {
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, stop event pump for a client. SessionId: {0}", client.SessionId));
                ((RemoteDesktopAbstractServiceProxy)client).IsActive = false;
            }
            else
            {
                // rossz sorrendben hív a kliens
                ServiceContract_Disconnected(client, new DisconnectEventArgs(client.SessionId));
            }
        }

        internal void SetImageClipQuality(IRemoteDesktopInternalClient client, Forge.RemoteDesktop.Contracts.ImageClipQualityArgs e)
        {
            bool allowToExecute = client.IsAuthenticated && ManagerState == ManagerStateEnum.Started;

            if (allowToExecute && mScreenClipsInitialized.WaitOne(5000))
            {
                if (e.QualityPercent == -1)
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_SERVICE_CONTRACT, image quality set to Auto. SessionId: {0}", client.SessionId));
                    if (client.ImageQuality == Settings.DefaultImageClipQuality) return;
                    client.ImageQuality = Settings.DefaultImageClipQuality;
                }
                else
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_SERVICE_CONTRACT, image quality set to {0}%. SessionId: {1}", e.QualityPercent.ToString(), client.SessionId));
                    if (client.ImageQuality == e.QualityPercent) return;
                    client.ImageQuality = e.QualityPercent;
                }

                lock (mLockObjectForClients)
                {
                    foreach (DesktopClip clip in client.SubscribedClips)
                    {
                        //clip.CRC = new byte[] { };
                        ClientContext clientContext = clip.SubscribedClients[client];
                        clientContext.IsRequestToResend = true;
                        if (client.IsActive)
                        {
                            clientContext.WorkerThreadEvent.Set();
                        }
                    }
                }
            }
            else
            {
                // rossz sorrendben hív a kliens
                ServiceContract_Disconnected(client, new DisconnectEventArgs(client.SessionId));
            }
        }

        internal void RefreshDesktop(IRemoteDesktopInternalClient client)
        {
            bool allowToExecute = client.IsAuthenticated && ManagerState == ManagerStateEnum.Started;

            if (allowToExecute && mScreenClipsInitialized.WaitOne(5000))
            {
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, client requested a desktop refresh. SessionId: {0}", client.SessionId));
                lock (mLockObjectForClients)
                {
                    foreach (DesktopClip clip in client.SubscribedClips)
                    {
                        //clip.CRC = new byte[] { };
                        ClientContext clientContext = clip.SubscribedClients[client];
                        clientContext.IsChanged = true;
                        if (client.IsActive)
                        {
                            clientContext.WorkerThreadEvent.Set();
                        }
                    }
                }
            }
            else
            {
                // rossz sorrendben hív a kliens
                ServiceContract_Disconnected(client, new DisconnectEventArgs(client.SessionId));
            }
        }

        internal void SendKeyEvent(IRemoteDesktopInternalClient client, KeyboardEventArgs e)
        {
            bool allowToExecute = client.IsAuthenticated && ManagerState == ManagerStateEnum.Started;

            if (allowToExecute && mScreenClipsInitialized.WaitOne(5000))
            {
                if (mConfigAcceptKeyboardAndMouseInputFromClients)
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, client sent a keyboard event. Key state: {0}, Key: {1}. SessionId: {2}", e.KeyEventType.ToString(), e.Key.ToString(), client.SessionId));
                    switch (e.KeyEventType)
                    {
                        case KeyboardEventTypeEnum.Down:
                            KeyboardOperations.PerformKeyDown(e.Key);
                            break;

                        case KeyboardEventTypeEnum.Up:
                            KeyboardOperations.PerformKeyUp(e.Key);
                            break;
                    }
                }
            }
            else
            {
                // rossz sorrendben hív a kliens
                ServiceContract_Disconnected(client, new DisconnectEventArgs(client.SessionId));
            }
        }

        internal void SendMouseButtonEvent(IRemoteDesktopInternalClient client, MouseButtonEventArgs e)
        {
            bool allowToExecute = client.IsAuthenticated && ManagerState == ManagerStateEnum.Started;

            if (allowToExecute && mScreenClipsInitialized.WaitOne(5000))
            {
                if (mConfigAcceptKeyboardAndMouseInputFromClients)
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, client sent a mouse button event. Button state: {0}, button: {1}. SessionId: {2}", e.EventType.ToString(), e.Button.ToString(), client.SessionId));
                    SetCursorPosOnService(client, e.Point);
                    switch (e.EventType)
                    {
                        case MouseButtonEventTypeEnum.Down:
                            MouseOperations.PerformDown(e.Button);
                            break;

                        case MouseButtonEventTypeEnum.Up:
                            MouseOperations.PerformUp(e.Button);
                            break;
                    }
                }
            }
            else
            {
                // rossz sorrendben hív a kliens
                ServiceContract_Disconnected(client, new DisconnectEventArgs(client.SessionId));
            }
        }

        internal void SendMouseWheelEvent(IRemoteDesktopInternalClient client, MouseWheelEventArgs e)
        {
            bool allowToExecute = client.IsAuthenticated && ManagerState == ManagerStateEnum.Started;

            if (allowToExecute && mScreenClipsInitialized.WaitOne(5000))
            {
                if (mConfigAcceptKeyboardAndMouseInputFromClients)
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, client sent a mouse wheel event. Wheel type: {0}, amount: {1}. SessionId: {2}", e.WheelType.ToString(), e.Amount.ToString(), client.SessionId));
                    switch (e.WheelType)
                    {
                        case MouseWheelTypeEnum.Vertical:
                            MouseOperations.MoveWheelVertically(e.Amount);
                            break;

                        case MouseWheelTypeEnum.Horizontal:
                            MouseOperations.MoveWheelHorizontally(e.Amount);
                            break;
                    }
                }
            }
            else
            {
                // rossz sorrendben hív a kliens
                ServiceContract_Disconnected(client, new DisconnectEventArgs(client.SessionId));
            }
        }

        internal void SendMouseMoveEvent(IRemoteDesktopInternalClient client, MouseMoveEventArgs e)
        {
            bool allowToExecute = client.IsAuthenticated && ManagerState == ManagerStateEnum.Started;

            if (allowToExecute && mScreenClipsInitialized.WaitOne(5000))
            {
                if (mConfigAcceptKeyboardAndMouseInputFromClients)
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, client sent a mouse button event. Location: {0}, {1}. SessionId: {2}", e.Position.X.ToString(), e.Position.Y.ToString(), client.SessionId));
                    SetCursorPosOnService(client, e.Position);
                }
            }
            else
            {
                // rossz sorrendben hív a kliens
                ServiceContract_Disconnected(client, new DisconnectEventArgs(client.SessionId));
            }
        }

        internal void SendClipboardContent(IRemoteDesktopInternalClient client, Forge.RemoteDesktop.Contracts.ClipboardChangedEventArgs e)
        {
            bool allowToExecute = client.IsAuthenticated && ManagerState == ManagerStateEnum.Started;

            if (allowToExecute && mScreenClipsInitialized.WaitOne(5000))
            {
                if (mConfigAcceptKeyboardAndMouseInputFromClients)
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, client sent a clipboard content. SessionId: {0}. Text: {1}", client.SessionId, e.Text));
                    if (string.Compare(client.ClipboardContent, e.Text) != 0)
                    {
                        client.ClipboardContent = e.Text;
                        mUIAccessor.SetClipboardContent(e.Text);
                    }
                }
            }
            else
            {
                // rossz sorrendben hív a kliens
                ServiceContract_Disconnected(client, new DisconnectEventArgs(client.SessionId));
            }
        }

        internal void SendFile(IRemoteDesktopInternalClient client, string fileName, System.IO.Stream file)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                ThrowHelper.ThrowArgumentNullException("fileName");
            }
            if (file == null)
            {
                ThrowHelper.ThrowArgumentNullException("file");
            }

            bool allowToExecute = client.IsAuthenticated && ManagerState == ManagerStateEnum.Started;

            if (allowToExecute && mScreenClipsInitialized.WaitOne(5000))
            {
                if (mConfigAcceptKeyboardAndMouseInputFromClients)
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, client sent a file. File name: {0}, size: {1}. SessionId: {2}", fileName, file.Length.ToString(), client.SessionId));
                    mUIAccessor.SaveFile(fileName, file);
                }
            }
            else
            {
                // rossz sorrendben hív a kliens
                ServiceContract_Disconnected(client, new DisconnectEventArgs(client.SessionId));
            }
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Registers to peer context.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="serviceDescriptor">The service descriptor.</param>
        protected override void RegisterToPeerContext(Channel channel, long priority, IServiceDescriptor serviceDescriptor)
        {
            if (Settings.PropagateServiceOnTheNetwork)
            {
                base.RegisterToPeerContext(channel, priority, serviceDescriptor);
            }
        }

        #endregion

        #region Private method(s)

        private void SetCursorPosOnService(IRemoteDesktopInternalClient client, Point mousePos)
        {
            client.LastMousePosX = mousePos.X;
            client.LastMousePosY = mousePos.Y;
            Point pos = new Point(mousePos.X + mScreenDataContainer.MinValues.X, mousePos.Y + mScreenDataContainer.MinValues.Y);
            Cursor.Position = pos;
            if (mScreenDataContainer != null)
            {
                mMouseMovePosition = pos;
                mMouseMoveEvent.Set();
            }
        }

        private void Settings_EventConfigurationChanged(object sender, EventArgs e)
        {
            ApplyConfiguration();
        }

        private void ApplyConfiguration()
        {
            mConfigAcceptKeyboardAndMouseInputFromClients = Settings.AcceptKeyboardAndMouseInputFromClients;
            mConfigDesktopImageClipHeight = Settings.DesktopImageClipHeight;
            mConfigDesktopImageClipWidth = Settings.DesktopImageClipWidth;
            mConfigDesktopShareMode = Settings.DesktopShareMode;
        }

        private void ServiceContract_Disconnected(object sender, DisconnectEventArgs e)
        {
            lock (mLockObjectForClients)
            {
                IRemoteDesktopInternalClient serviceContract = (IRemoteDesktopInternalClient)sender;
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, client disconnected. SessionId: {0}", e.SessionId));
                serviceContract.Disconnected -= new EventHandler<DisconnectEventArgs>(ServiceContract_Disconnected);

                mClients.Remove(serviceContract.SessionId);
                mClientsWatchForTimeout.Remove(serviceContract.SessionId);
                mClientsAccepteds.Remove(serviceContract.SessionId);
                serviceContract.IsActive = false;
                serviceContract.IsAccepted = false;

                if (mClientVsThreadClients.ContainsKey(serviceContract))
                {
                    ThreadClientContext container = mClientVsThreadClients[serviceContract];
                    container.Clients.Remove(serviceContract);
                    container.WorkEvent.Set();
                }

                foreach (DesktopClip clip in serviceContract.SubscribedClips)
                {
                    clip.SubscribedClients.Remove(serviceContract);
                }
                serviceContract.SubscribedClips.Clear();

                RaiseEvent(EventConnectionStateChange, this, new ConnectionStateChangedEventArgs(serviceContract, false));

                serviceContract.Dispose();
            }
        }

        private void TimeoutWatchThreadMain()
        {
            if (LOGGER.IsDebugEnabled) LOGGER.Debug("REMOTE_DESKTOP_MANAGER, timeout watch thread started.");
            while (true)
            {
                List<IRemoteDesktopInternalClient> clientsToDisconnect = new List<IRemoteDesktopInternalClient>();
                int maxWait = Timeout.Infinite;

                lock (mLockObjectForClients)
                {
                    long currentTicks = DateTime.Now.Ticks;
                    foreach (KeyValuePair<string, TimeoutWatch> kv in mClientsWatchForTimeout)
                    {
                        DateTime timeToDisconnect = kv.Value.Connected.AddMilliseconds(Settings.LoginTimeoutInMs);
                        if (timeToDisconnect.Ticks <= currentTicks)
                        {
                            clientsToDisconnect.Add(kv.Value.Service);
                        }
                        else if (maxWait == Timeout.Infinite || maxWait > (timeToDisconnect.Ticks - currentTicks))
                        {
                            maxWait = Convert.ToInt32(timeToDisconnect.Ticks - currentTicks);
                        }
                    }
                }

                clientsToDisconnect.ForEach(i => i.Dispose());
                clientsToDisconnect.Clear();

                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, timeout watch thread waiting for {0} ms...", maxWait.ToString()));
                mTimeoutWatchEvent.WaitOne(maxWait);
            }
        }

        private void ScreenCaptureThreadMain()
        {
            if (LOGGER.IsDebugEnabled) LOGGER.Debug("REMOTE_DESKTOP_MANAGER, starting screen capture thread.");
            {
                // 1. lépés: X és Y koordináták számítása
                int minX = Screen.AllScreens.Select(i => i.Bounds.X).Min();
                int minY = Screen.AllScreens.Select(i => i.Bounds.Y).Min();
                int maxX = Screen.AllScreens.Select(i => i.Bounds.Right).Max();
                int maxY = Screen.AllScreens.Select(i => i.Bounds.Y + i.Bounds.Height).Max();

                int width = maxX - minX;
                int height = maxY - minY;

                // 2. lépés: clip-ek legyártása
                int clipNumberX = Convert.ToInt32(Math.Ceiling((decimal)width / Settings.DesktopImageClipWidth));
                int clipNumberY = Convert.ToInt32(Math.Ceiling((decimal)height / Settings.DesktopImageClipHeight));

                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, clip numbers. X: {0}, Y: {1}.", clipNumberX.ToString(), clipNumberY.ToString()));

                // 3. lépés: clipek feltöltése kezdő képi információkkal
                Point minValues = new Point(minX, minY);
                Size screenSize = new Size(width, height);
                DesktopClip[,] clips = new DesktopClip[0, 0];
                Bitmap background = null;

                try
                {
                    background = GetScreen(minValues, screenSize);

                    clips = new DesktopClip[clipNumberX, clipNumberY];
                    for (int indexX = 0; indexX < clips.GetLength(0); indexX++)
                    {
                        for (int indexY = 0; indexY < clips.GetLength(1); indexY++)
                        {
                            int x = indexX * Settings.DesktopImageClipWidth;
                            int y = indexY * Settings.DesktopImageClipHeight;
                            int sizeX = x + Settings.DesktopImageClipWidth > width ? width - x : Settings.DesktopImageClipWidth;
                            int sizeY = y + Settings.DesktopImageClipHeight > height ? height - y : Settings.DesktopImageClipHeight;
                            clips[indexX, indexY] = new DesktopClip(new Point(x, y), new Size(sizeX, sizeY));
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("REMOTE_DESKTOP_MANAGER, failed to capture screen. Reason: {0}", ex.Message));
                }

                mScreenDataContainer = new ScreenDataContainer(minValues, screenSize, clips);
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, screen data. Screens: {0}; MinValues: {1}, {2}; desktop size: {3}, {4}; Clip dim sizes: {5}, {6}.", Screen.AllScreens.Length.ToString(), minValues.X.ToString(), minValues.Y.ToString(), screenSize.Width.ToString(), screenSize.Height.ToString(), clips.GetLength(0).ToString(), clips.GetLength(1).ToString()));

                // 4. lépés: clippekbe bedugás
                if (background != null)
                {
                    FillDesktopClips(background, clips);
                }

                mScreenClipsInitialized.Set(); // ha van várakozó kliens, most futhat tovább
            }
            while (ManagerState == ManagerStateEnum.Started)
            {
                bool subscribed = false;
                while (mClientsAccepteds.Count > 0)
                {
                    // 1. kép lopása
                    // 2. clip-ek feltöltése/adminja
                    // 3. mouse, keyboard és clipboard eseményekre feliratkozás
                    if (!subscribed)
                    {
                        MouseEventHookManager.Instance.MouseMoveExtended += new EventHandler<MouseEventExtendedArgs>(Instance_MouseMoveExtended);
                        ClipboardEventManager.Instance.EventClipboardChanged += new EventHandler<Native.Hooks.ClipboardChangedEventArgs>(Instance_EventClipboardChanged);
                        if (LOGGER.IsDebugEnabled) LOGGER.Debug("REMOTE_DESKTOP_MANAGER, subscribed for mouse move and clipboard change events.");
                        subscribed = true;
                    }

                    int sleep = 0;
                    try
                    {
                        Bitmap background = GetScreen(mScreenDataContainer.MinValues, mScreenDataContainer.ScreenSize);
                        sleep = FillDesktopClips(background, mScreenDataContainer.DesktopClips);
                        sleep = mScreenCapturePollInterval - sleep;
                        if (sleep < 0) sleep = 0;
                    }
                    catch (Exception ex)
                    {
                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("REMOTE_DESKTOP_MANAGER, failed to capture screen. Reason: {0}", ex.Message));
                    }

                    Thread.Sleep(sleep);
                }
                MouseEventHookManager.Instance.MouseMoveExtended -= new EventHandler<MouseEventExtendedArgs>(Instance_MouseMoveExtended);
                ClipboardEventManager.Instance.EventClipboardChanged -= new EventHandler<Native.Hooks.ClipboardChangedEventArgs>(Instance_EventClipboardChanged);
                subscribed = false;
                if (LOGGER.IsDebugEnabled) LOGGER.Debug("REMOTE_DESKTOP_MANAGER, screen capture thread going to sleep, no accepted clients.");
                mScreenCaptureIdleEvent.WaitOne();
            }

            // legközelebb addig nem jöhet be client, amíg nincs inicializálva a clips
            mScreenClipsInitialized.Reset();
            if (LOGGER.IsDebugEnabled) LOGGER.Debug("REMOTE_DESKTOP_MANAGER, screen capture thread stopped.");
        }

        private void Instance_EventClipboardChanged(object sender, Native.Hooks.ClipboardChangedEventArgs e)
        {
            if (e.ContainsText())
            {
                string text = e.GetText();
                foreach (IRemoteDesktopInternalClient client in GetAcceptedClients())
                {
                    if (client.IsActive && string.Compare(client.ClipboardContent, text) != 0)
                    {
                        try
                        {
                            if (LOGGER.IsDebugEnabled) LOGGER.Debug("REMOTE_DESKTOP_MANAGER, sending clipboard content to a client.");
                            client.ClipboardContent = text;
                            client.ServiceSendClipboardContent(new Contracts.ClipboardChangedEventArgs(text));
                        }
                        catch (Exception ex)
                        {
                            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, failed to send clipboard content to a client. Reason: {0}", ex.Message));
                        }
                    }
                }
            }
        }

        private List<IRemoteDesktopInternalClient> GetAcceptedClients()
        {
            lock (mLockObjectForClients)
            {
                return new List<IRemoteDesktopInternalClient>(mClientsAccepteds.Values);
            }
        }

        private void MouseMoveSenderThreadMain()
        {
            if (LOGGER.IsDebugEnabled) LOGGER.Debug("REMOTE_DESKTOP_MANAGER, mouse move sender thread started.");
            while (ManagerState == ManagerStateEnum.Started)
            {
                mMouseMoveEvent.WaitOne();

                Point pos = mMouseMovePosition;
                Cursor cursor = mUIAccessor.GetCurrentCursor();
                if (cursor != null)
                {
                    if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, current cursor is {0}.", cursor.ToString()));
                }

                foreach (IRemoteDesktopInternalClient client in GetAcceptedClients())
                {
                    if (client.IsActive &&
                        (client.LastMousePosX != pos.X || client.LastMousePosY != pos.Y ||
                        (cursor != null && string.Compare(client.LastCursorId, mCursorWithIds[cursor]) != 0) ||
                        (cursor == null && !string.IsNullOrEmpty(client.LastCursorId))))
                    {
                        try
                        {
                            string cursorId = cursor == null ? string.Empty : mCursorWithIds[cursor];
                            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, sending mouse move location to a client. X: {0}, Y: {1}, CursorId: {2}", pos.X.ToString(), pos.Y.ToString(), cursorId));
                            client.LastMousePosX = pos.X;
                            client.LastMousePosY = pos.Y;
                            client.LastCursorId = cursorId;
                            client.ServiceSendMouseMoveEvent(new MouseMoveServiceEventArgs(pos, cursorId));
                        }
                        catch (Exception ex)
                        {
                            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, failed to send mouse location to a client. Reason: {0}", ex.Message));
                        }
                    }
                }

                Thread.Sleep(Settings.MouseMoveSendInterval);
            }
            if (LOGGER.IsDebugEnabled) LOGGER.Debug("REMOTE_DESKTOP_MANAGER, mouse move sender thread stopped.");
        }

        private void Instance_MouseMoveExtended(object sender, MouseEventExtendedArgs e)
        {
            if (mScreenDataContainer != null)
            {
                mMouseMovePosition = new Point(mScreenDataContainer.MinValues.X + e.Location.X, mScreenDataContainer.MinValues.Y + e.Location.Y);
                mMouseMoveEvent.Set();
            }
        }

        private static Size GetDesktopSize()
        {
            Size result = Size.Empty;

            int width = 0;
            int height = 0;
            foreach (Screen scr in Screen.AllScreens)
            {
                int x = Math.Abs(scr.Bounds.Location.X);
                x = x + scr.Bounds.Width;

                int y = Math.Abs(scr.Bounds.Location.Y);
                y = y + scr.Bounds.Height;

                if (width < x)
                {
                    width = x;
                }
                if (height < y)
                {
                    height = y;
                }
            }

            return new Size(width, height);
        }

        private static CursorInfo[] GetCursors()
        {
            List<CursorInfo> result = new List<CursorInfo>();

            foreach (PropertyInfo pi in typeof(Cursors).GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                MethodInfo mi = pi.GetGetMethod();
                if (mi != null)
                {
                    Cursor c = mi.Invoke(null, new object[] { }) as Cursor;
                    if (c != null)
                    {
                        result.Add(new CursorInfo(pi.Name, c));
                        if (!mCursorWithIds.ContainsKey(c))
                        {
                            mCursorWithIds[c] = pi.Name;
                        }
                    }
                }
            }

            return result.ToArray();
        }

        private static Bitmap GetScreen(Point minValues, Size screenSize)
        {
            int minX = minValues.X;
            int minY = minValues.Y;
            int width = screenSize.Width;
            int height = screenSize.Height;
            Bitmap background = new Bitmap(width, height);

            //LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, (get screen), width: {0}, height: {1}", width.ToString(), height.ToString()));

            using (Graphics backgroundGraphics = Graphics.FromImage(background))
            {
                backgroundGraphics.DrawRectangle(new Pen(Color.Black), 0, 0, width, height);

                foreach (Screen scr in Screen.AllScreens)
                {
                    Size size = scr.Bounds.Size; // full screen
                    backgroundGraphics.CopyFromScreen(scr.Bounds.Location, new Point(scr.Bounds.Location.X - minX, scr.Bounds.Location.Y - minY), size);
                }
            }

            //using (FileStream fs = new FileStream("C:\\test.png", FileMode.Create))
            //{
            //    SaveImage(fs, background, 100);
            //}

            return background;
        }

        private int FillDesktopClips(Bitmap background, DesktopClip[,] clips)
        {
            Bitmap clipImage = null;
            Graphics g = null;
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                for (int indexY = 0; indexY < clips.GetLength(1); indexY++)
                {
                    for (int indexX = 0; indexX < clips.GetLength(0); indexX++)
                    {
                        DesktopClip clip = clips[indexX, indexY];
                        if (clipImage == null || clipImage.Height != clip.Size.Height || clipImage.Width != clip.Size.Width)
                        {
                            if (clipImage != null)
                            {
                                clipImage.Dispose();
                            }
                            if (g != null)
                            {
                                g.Dispose();
                            }
                            clipImage = new Bitmap(clip.Size.Width, clip.Size.Height);
                            g = Graphics.FromImage(clipImage);
                        }
                        g.DrawImage(background, new Rectangle(Point.Empty, clip.Size), new Rectangle(clip.Location, clip.Size), GraphicsUnit.Pixel);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            SaveImage(ms, clipImage, Settings.DefaultImageClipQuality);
                            byte[] imageData = ms.ToArray();
                            ms.SetLength(0);
                            byte[] crc = CreateCRC(imageData);
                            if (clip.LastContent == null)
                            {
                                clip.LastContent = imageData;
                                clip.CRC = crc;
                            }
                            else if (!Arrays.DeepEquals(crc, clip.CRC))
                            {
                                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, desktop clip content changed. X: {0}, Y: {1}.", clip.Location.X.ToString(), clip.Location.Y.ToString()));
                                SaveImage(ms, clipImage, 100); // save the max quality
                                clip.MaxQuality = ms.ToArray();
                                ms.SetLength(0);
                                clip.LastContent = imageData;
                                clip.CRC = crc;
                                foreach (ClientContext context in clip.SubscribedClients.Values)
                                {
                                    context.IsChanged = true;
                                    context.IsRequestToResend = false;
                                    if (context.Client.ImageQuality == Settings.DefaultImageClipQuality)
                                    {
                                        context.ContentToSend = imageData;
                                    }
                                    else
                                    {
                                        SaveImage(ms, clipImage, context.Client.ImageQuality);
                                        context.ContentToSend = ms.ToArray();
                                        ms.SetLength(0);
                                    }
                                    if (context.Client.IsActive)
                                    {
                                        context.WorkerThreadEvent.Set(); // meglökjük a feldolgozó szálat
                                    }
                                }
                            }
                            else
                            {
                                // screen minőség állítás volt-e?
                                foreach (ClientContext context in clip.SubscribedClients.Values)
                                {
                                    if (context.IsRequestToResend)
                                    {
                                        context.IsRequestToResend = false;
                                        context.IsChanged = true;
                                        if (context.Client.ImageQuality == Settings.DefaultImageClipQuality)
                                        {
                                            context.ContentToSend = imageData;
                                        }
                                        else
                                        {
                                            SaveImage(ms, clipImage, context.Client.ImageQuality);
                                            context.ContentToSend = ms.ToArray();
                                            ms.SetLength(0);
                                        }
                                        if (context.Client.IsActive)
                                        {
                                            context.WorkerThreadEvent.Set(); // meglökjük a feldolgozó szálat
                                        }
                                    }
                                }
                            }
                            ms.SetLength(0);
                        }
                    }
                }
            }
            finally
            {
                if (clipImage != null)
                {
                    clipImage.Dispose();
                }
                if (g != null)
                {
                    g.Dispose();
                }
                sw.Stop();
                if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_DESKTOP_MANAGER, screen captured. Takes: {0} ms.", sw.ElapsedMilliseconds.ToString()));
            }

            return Convert.ToInt32(sw.ElapsedMilliseconds);
        }

        private static byte[] CreateCRC(byte[] bitmapData)
        {
            return mCrcCalculator.ComputeHash(bitmapData);
        }

        private static ImageCodecInfo GetImageEncoder(ImageFormat format)
        {
            if (mImageCodecInfoJpg == null)
            {
                foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
                {
                    if (codec.FormatID == format.Guid)
                    {
                        mImageCodecInfoJpg = codec;
                        break;
                    }
                }
            }
            return mImageCodecInfoJpg;
        }

        private static void SaveImage(Stream ms, byte[] bitmapData, int quality)
        {
            using (MemoryStream stream = new MemoryStream(bitmapData))
            {
                stream.Position = 0;
                SaveImage(ms, (Bitmap)Bitmap.FromStream(stream), quality);
            }
        }

        private static void SaveImage(Stream ms, Bitmap bitmap, int quality)
        {
            if (quality >= 95)
            {
                bitmap.Save(ms, ImageFormat.Png);
            }
            else
            {
                ImageCodecInfo jpgEncoder = GetImageEncoder(ImageFormat.Jpeg);
                System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;

                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(encoder, Convert.ToInt64(quality));

                bitmap.Save(ms, jpgEncoder, encoderParameters);
            }
        }

        private void ClientServiceThreadMain()
        {
            if (LOGGER.IsDebugEnabled) LOGGER.Debug("REMOTE_DESKTOP_MANAGER, starting client service thread.");

            ThreadClientContext context = null;
            lock (mLockObjectForClients)
            {
                foreach (ThreadClientContext c in mWorkerThreadContainers)
                {
                    if (c.Thread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId)
                    {
                        context = c;
                        break;
                    }
                }
            }

            while (true)
            {
                HashSet<IRemoteDesktopInternalClient> clients = null;
                lock (mLockObjectForClients)
                {
                    if (context.Clients.Count == 0)
                    {
                        mWorkerThreadContainers.Remove(context);
                        break;
                    }
                    else
                    {
                        clients = new HashSet<IRemoteDesktopInternalClient>(context.Clients);
                    }
                }

                foreach (IRemoteDesktopInternalClient client in clients)
                {
                    if (client.IsActive)
                    {
                        HashSet<DesktopClip> clips = null;
                        lock (mLockObjectForClients)
                        {
                            clips = new HashSet<DesktopClip>(client.SubscribedClips);
                        }
                        foreach (DesktopClip clip in clips)
                        {
                            ClientContext clientContext = null;
                            lock (mLockObjectForClients)
                            {
                                if (clip.SubscribedClients.ContainsKey(client))
                                {
                                    clientContext = clip.SubscribedClients[client];
                                }
                            }
                            if (clientContext != null)
                            {
                                if (clientContext.IsChanged)
                                {
                                    clientContext.IsChanged = false;
                                    try
                                    {
                                        client.ServiceSendDesktopImageClip(new DesktopImageClipArgs(clip.Location, clientContext.ContentToSend));
                                    }
                                    catch (Exception ex)
                                    {
                                        if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("RemoteDesktopServiceManager, failed to send desktop clip refresh. Reason: {0}", ex.Message));
                                    }
                                }
                            }
                        }
                    }
                }

                context.WorkEvent.WaitOne();
            }

            if (LOGGER.IsDebugEnabled) LOGGER.Debug("REMOTE_DESKTOP_MANAGER, client service thread stopped.");
        }

        private ThreadClientContext GetThreadClientContext(IRemoteDesktopInternalClient client)
        {
            ThreadClientContext container = mClientVsThreadClients.ContainsKey(client) ? mClientVsThreadClients[client] : null;
            if (container == null)
            {
                foreach (ThreadClientContext c in mWorkerThreadContainers)
                {
                    if (c.Clients.Count < Settings.ClientsPerServiceThreads)
                    {
                        c.Clients.Add(client);
                        container = c;
                        break;
                    }
                }
            }

            if (container == null)
            {
                if (LOGGER.IsDebugEnabled) LOGGER.Debug("REMOTE_DESKTOP_MANAGER, creating new thread client context.");
                Thread th = new Thread(new ThreadStart(ClientServiceThreadMain));
                th.Name = "RemoteDesktopClientWorker";
                th.IsBackground = true;

                container = new ThreadClientContext(th);
                container.Clients.Add(client);
                mClientVsThreadClients[client] = container;
                mWorkerThreadContainers.Add(container);

                th.Start();
            }

            return container;
        }

        #endregion

        #region Nested type(s)

        private class TimeoutWatch
        {

            /// <summary>
            /// Initializes a new instance of the <see cref="TimeoutWatch"/> class.
            /// </summary>
            /// <param name="service">The service.</param>
            public TimeoutWatch(IRemoteDesktopInternalClient service)
            {
                Service = service;
                Connected = DateTime.Now;
            }

            /// <summary>
            /// Gets or sets the service.
            /// </summary>
            /// <value>
            /// The service.
            /// </value>
            public IRemoteDesktopInternalClient Service { get; private set; }

            /// <summary>
            /// Gets or sets the connected.
            /// </summary>
            /// <value>
            /// The connected.
            /// </value>
            public DateTime Connected { get; private set; }

        }

        private class ScreenDataContainer
        {

            /// <summary>
            /// Initializes a new instance of the <see cref="ScreenDataContainer" /> class.
            /// </summary>
            /// <param name="minValues">The minimum values.</param>
            /// <param name="screenSize">Size of the screen.</param>
            /// <param name="desktopClips">The desktop clips.</param>
            public ScreenDataContainer(Point minValues, Size screenSize, DesktopClip[,] desktopClips)
            {
                if (desktopClips == null)
                {
                    ThrowHelper.ThrowArgumentNullException("desktopClips");
                }

                MinValues = minValues;
                ScreenSize = screenSize;
                DesktopClips = desktopClips;
            }

            /// <summary>
            /// Gets or sets the minimum values.
            /// </summary>
            /// <value>
            /// The minimum values.
            /// </value>
            public Point MinValues { get; private set; }

            /// <summary>
            /// Gets or sets the size of the screen.
            /// </summary>
            /// <value>
            /// The size of the screen.
            /// </value>
            public Size ScreenSize { get; private set; }

            /// <summary>
            /// Gets or sets the desktop clips.
            /// </summary>
            /// <value>
            /// The desktop clips.
            /// </value>
            public DesktopClip[,] DesktopClips { get; private set; }

        }

        private class ThreadClientContext
        {

            /// <summary>
            /// Initializes a new instance of the <see cref="ThreadClientContext"/> class.
            /// </summary>
            /// <param name="workerThread">The worker thread.</param>
            internal ThreadClientContext(Thread workerThread)
            {
                Thread = workerThread;
                WorkEvent = new AutoResetEvent(false);
                Clients = new HashSet<IRemoteDesktopInternalClient>();
            }

            /// <summary>
            /// Gets or sets the thread.
            /// </summary>
            /// <value>
            /// The thread.
            /// </value>
            internal Thread Thread { get; private set; }

            /// <summary>
            /// Gets or sets the work event.
            /// </summary>
            /// <value>
            /// The work event.
            /// </value>
            internal AutoResetEvent WorkEvent { get; private set; }

            /// <summary>
            /// Gets or sets the clients.
            /// </summary>
            /// <value>
            /// The clients.
            /// </value>
            internal HashSet<IRemoteDesktopInternalClient> Clients { get; private set; }

        }

        private class UIAccessorControl : UserControl
        {

            private ILog LOGGER = null;

            /// <summary>
            /// Initializes a new instance of the <see cref="UIAccessorControl"/> class.
            /// </summary>
            internal UIAccessorControl(ILog logger)
            {
                LOGGER = logger;
            }

            /// <summary>
            /// Sets the content of the clipboard.
            /// </summary>
            /// <param name="text">The text.</param>
            internal void SetClipboardContent(string text)
            {
                if (InvokeRequired)
                {
                    SetClipboardContentHandler d = new SetClipboardContentHandler(SetClipboardContent);
                    ((UIAccessorControl)d.Target).Invoke(d, text);
                    return;
                }

                Clipboard.SetText(text);
            }

            /// <summary>
            /// Gets the current cursor.
            /// </summary>
            /// <returns></returns>
            internal Cursor GetCurrentCursor()
            {
                if (InvokeRequired)
                {
                    GetCurrentCursorHandler d = new GetCurrentCursorHandler(GetCurrentCursor);
                    return ((UIAccessorControl)d.Target).Invoke(d) as Cursor;
                }

                return GetCursor();
            }

            private Cursor GetCursor()
            {
                Cursor result = null;

                Forge.Native.Structures.CursorInfo ci;
                ci.cbSize = Marshal.SizeOf(typeof(Forge.Native.Structures.CursorInfo));

                if (NativeMethods.GetCursorInfo(out ci))
                {
                    if (!ci.hCursor.Equals(IntPtr.Zero))
                    {
                        foreach (Cursor c in mCursorWithIds.Keys)
                        {
                            if (c.Handle.Equals(ci.hCursor))
                            {
                                result = c;
                                break;
                            }
                        }
                    }
                }

                return result;
            }

            internal void SaveFile(string fileName, Stream file)
            {
                if (InvokeRequired)
                {
                    SaveFileHandler d = new SaveFileHandler(SaveFile);
                    ((UIAccessorControl)d.Target).Invoke(d, fileName, file);
                    return;
                }

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.FileName = fileName;
                    sfd.Filter = "All Files (*.*)|*.*";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                            {
                                byte[] data = new byte[4096];
                                int readedBytes = file.Read(data, 0, data.Length);
                                while (readedBytes > 0)
                                {
                                    fs.Write(data, 0, readedBytes);
                                    readedBytes = file.Read(data, 0, data.Length);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (LOGGER.IsErrorEnabled) LOGGER.Error(string.Format("Failed to save file: {0}", sfd.FileName), ex);
                            MessageBox.Show(ex.Message, "Failed to save file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }

        }

        #endregion

    }

}
