/* *********************************************************************
 * Date: 06 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using Forge.Collections;
using Forge.EventRaiser;
using Forge.Logging;
using Forge.Native.Hooks;
using Forge.Net.Remoting.Proxy;
using Forge.Net.Services.Locators;
using Forge.Net.Synapse;
using Forge.RemoteDesktop.Contracts;

namespace Forge.RemoteDesktop.Client
{

    internal delegate void DrawDesktopClipHandler(DesktopImageClipArgs e);
    internal delegate void MouseWheelInternalHandler(RemoteDesktopWinFormsControl.MouseWheelInternalEventArgs e);
    internal delegate void SetClipboardContentHandler(string text);

    /// <summary>
    /// Represents a client terminal window for Winforms application
    /// </summary>
    public sealed partial class RemoteDesktopWinFormsControl : UserControl
    {

        #region Field(s)

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(RemoteDesktopWinFormsControl));

        private static readonly IRemoteServiceLocator<IRemoteDesktop> mRemoteServiceLocator = RemoteServiceLocatorManager.GetServiceLocator<IRemoteDesktop, RemoteDesktopServiceLocator>();

        private IRemoteDesktopClientInternal mProxy = null;

        private DescriptionResponseArgs mServiceConfig = null;

        private static readonly Dictionary<string, Cursor> mCursorWithIds = new Dictionary<string, Cursor>();

        private Bitmap mRemoteDesktopImage = null;

        private Area mVisibleArea = null;

        private Thread mMouseMoveThread = null;

        private bool mClientMouseMove = false;

        private readonly AutoResetEvent mMouseMoveWaitEvent = new AutoResetEvent(false);

        private readonly object mLockEventObject = new object();

        private const int MK_LBUTTON = 0x1;
        private const int MK_RBUTTON = 0x2;
        private const int MK_MBUTTON = 0x10;

        private readonly Dictionary<KeysSubscription, ListSpecialized<EventHandler<SubscribedKeyPressEventArgs>>> mKeysSubscriptions = new Dictionary<KeysSubscription, ListSpecialized<EventHandler<SubscribedKeyPressEventArgs>>>();

        /// <summary>
        /// Occurs when [event connection state change].
        /// </summary>
        public event EventHandler<ConnectionStateChangeEventArgs> EventConnectionStateChange;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDesktopWinFormsControl"/> class.
        /// </summary>
        public RemoteDesktopWinFormsControl()
        {
            InitializeComponent();

            mServiceConfig = new DescriptionResponseArgs(pbClient.Size, Size.Empty, new CursorInfo[] { }, false);
            DrawInitialBlackBackground();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the known services on the network.
        /// </summary>
        /// <value>
        /// The services.
        /// </value>
        public static IListSpecialized<ServiceProvider> Services
        {
            get
            {
                return mRemoteServiceLocator.AvailableServiceProviders;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [is connected].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is connected]; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get { return mProxy != null && mProxy.IsConnected ? true : false; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [is authenticated].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is authenticated]; otherwise, <c>false</c>.
        /// </value>
        public bool IsAuthenticated
        {
            get { return mProxy != null && mProxy.IsConnected && mProxy.IsAuthenticated ? true : false; }
        }

        /// <summary>
        /// Gets a value indicating whether [is active].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is active]; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive
        {
            get { return mProxy != null && mProxy.IsConnected && mProxy.IsAuthenticated && mProxy.IsActive ? true : false; }
        }

        /// <summary>
        /// Gets a value indicating whether [is event pump active].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is event pump active]; otherwise, <c>false</c>.
        /// </value>
        public bool IsEventPumpActive
        {
            get
            {
                return mProxy == null ? false : mProxy.IsActive;
            }
        }

        /// <summary>
        /// Gets the session unique identifier.
        /// </summary>
        /// <value>
        /// The session unique identifier.
        /// </value>
        public string SessionId
        {
            get { return mProxy == null ? string.Empty : mProxy.SessionId; }
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Connects the specified end point.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="endPoint">The end point.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Connect(string channelId, AddressEndPoint endPoint)
        {
            DoDisposeCheck();
            if (string.IsNullOrEmpty(channelId))
            {
                ThrowHelper.ThrowArgumentNullException("channelId");
            }
            if (endPoint == null)
            {
                ThrowHelper.ThrowArgumentNullException("endPoint");
            }

            Disconnect();

            ProxyFactory<IRemoteDesktop> factory = new ProxyFactory<IRemoteDesktop>(channelId, endPoint);
            lock (mLockEventObject)
            {
                mProxy = (IRemoteDesktopClientInternal)factory.CreateProxy();
                mProxy.Owner = this;
                Raiser.CallDelegatorBySync(EventConnectionStateChange, new object[] { this, new ConnectionStateChangeEventArgs(true) });
                mProxy.Disconnected += new EventHandler<DisconnectEventArgs>(Proxy_Disconnected);
            }
        }

        /// <summary>
        /// Disconnects this instance from the service.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Disconnect()
        {
            if (mProxy != null)
            {
                mProxy.Dispose();
                Proxy_Disconnected(null, null);
                mProxy = null;
                DrawInitialBlackBackground();
            }
        }

        /// <summary>
        /// Gets the authentication mode.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Client was not connected.</exception>
        public AuthenticationModeEnum GetAuthenticationMode()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Client was not connected.");
            }

            return mProxy.GetAuthenticationInfo().AuthenticationMode;
        }

        /// <summary>
        /// Logins with the specified username and password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Client was not connected.</exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public LoginResponseStateEnum Login(string username, string password)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Client was not connected.");
            }

            LoginResponseArgs response = mProxy.Login(new LoginRequestArgs(username, password));
            LoginResponseStateEnum result = LoginResponseStateEnum.AccessDenied;
            if (response != null)
            {
                result = response.ResponseState;
                if (result == LoginResponseStateEnum.AccessGranted)
                {
                    mProxy.IsAuthenticated = true;
                    mServiceConfig = mProxy.ClientGetConfiguration();
                    mCursorWithIds.Clear();
                    foreach (CursorInfo info in mServiceConfig.Cursors)
                    {
                        mCursorWithIds[info.CursorId] = info.Cursor;
                    }
                    pbClient.Size = mServiceConfig.DesktopSize;
                    DrawInitialBlackBackground();
                }
            }
            return result;
        }

        /// <summary>
        /// Starts the event pump.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// Client was not connected.
        /// or
        /// Client was not authenticated.
        /// </exception>
        public void StartEventPump()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Client was not connected.");
            }
            if (!IsAuthenticated)
            {
                throw new InvalidOperationException("Client was not authenticated.");
            }

            if (this.InvokeRequired)
            {
                Action d = new Action(StartEventPump);
                ((RemoteDesktopWinFormsControl)d.Target).Invoke(d);
                return;
            }

            // innentől UI thread

            if (mProxy != null && !mProxy.IsActive)
            {
                mVisibleArea = GetVisibleArea();

                mProxy.IsActive = true;
                mProxy.ClientSubscribeForDesktop(mVisibleArea);
                MouseMoveServiceEventArgs mouseData = mProxy.ClientStartEventPump();
                mProxy.LastCursorId = mouseData.CursorId;
                mProxy.LastMousePosX = mouseData.Position.X;
                mProxy.LastMousePosY = mouseData.Position.Y;
                ClipboardEventManager.Instance.EventClipboardChanged += new EventHandler<Native.Hooks.ClipboardChangedEventArgs>(ClipboardChangedEventHandler);
                ClipboardEventManager.Instance.Start();
                DrawMouse();
                if (mServiceConfig.AcceptKeyboardAndMouseInputs)
                {
                    SubscribeEvents();
                    mMouseMoveThread = new Thread(new ThreadStart(MouseMoveMain));
                    mMouseMoveThread.IsBackground = true;
                    mMouseMoveThread.Name = "MouseMoveSenderClient";
                    mMouseMoveThread.Start();
                }
            }
        }

        /// <summary>
        /// Stops the event pump.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// Client was not connected.
        /// or
        /// Client was not authenticated.
        /// </exception>
        public void StopEventPump()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Client was not connected.");
            }
            if (!IsAuthenticated)
            {
                throw new InvalidOperationException("Client was not authenticated.");
            }

            if (mProxy.IsActive)
            {
                ClipboardEventManager.Instance.EventClipboardChanged -= new EventHandler<Native.Hooks.ClipboardChangedEventArgs>(ClipboardChangedEventHandler);
                if (mServiceConfig.AcceptKeyboardAndMouseInputs)
                {
                    UnsubscribeEvents();
                }
                mProxy.ClientStopEventPump();
                mProxy.IsActive = false;
            }
        }

        /// <summary>
        /// Refreshes the desktop.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// Client was not connected.
        /// or
        /// Client was not authenticated.
        /// or
        /// Client is not active.
        /// </exception>
        public void RefreshDesktop()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Client was not connected.");
            }
            if (!IsAuthenticated)
            {
                throw new InvalidOperationException("Client was not authenticated.");
            }
            if (!IsActive)
            {
                throw new InvalidOperationException("Client is not active.");
            }

            mProxy.ClientRefreshDesktop();
        }

        /// <summary>
        /// Sets the desktop quality manually.
        /// </summary>
        /// <param name="qualityPercent">The quality percent.</param>
        /// <exception cref="System.InvalidOperationException">
        /// Client was not connected.
        /// or
        /// Client was not authenticated.
        /// </exception>
        public void SetDesktopQualityManually(int qualityPercent)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Client was not connected.");
            }
            if (!IsAuthenticated)
            {
                throw new InvalidOperationException("Client was not authenticated.");
            }
            if (qualityPercent != -1 && (qualityPercent > 100 || qualityPercent < 10))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("qualityPercent");
            }

            mProxy.ClientSetImageClipQuality(new ImageClipQualityArgs(qualityPercent));
        }

        /// <summary>
        /// Sends the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="stream">The stream.</param>
        /// <exception cref="System.InvalidOperationException">
        /// Client was not connected.
        /// or
        /// Client was not authenticated.
        /// or
        /// Client is not active.
        /// </exception>
        public void SendFile(string fileName, Stream stream)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Client was not connected.");
            }
            if (!IsAuthenticated)
            {
                throw new InvalidOperationException("Client was not authenticated.");
            }

            mProxy.ClientSendFile(fileName, stream);
        }

        /// <summary>
        /// Subscribes for keys.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <param name="e">The decimal.</param>
        public void SubscribeForKeys(Keys keys, EventHandler<SubscribedKeyPressEventArgs> e)
        {
            DoDisposeCheck();

            if (e == null)
            {
                ThrowHelper.ThrowArgumentNullException("e");
            }

            SubscribeForKeys(new KeysSubscription(keys), e);
        }

        /// <summary>
        /// Subscribes for keys.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        /// <param name="e">The decimal.</param>
        public void SubscribeForKeys(KeysSubscription subscription, EventHandler<SubscribedKeyPressEventArgs> e)
        {
            DoDisposeCheck();

            if (subscription == null)
            {
                ThrowHelper.ThrowArgumentNullException("subscription");
            }
            if (e == null)
            {
                ThrowHelper.ThrowArgumentNullException("e");
            }

            lock (mKeysSubscriptions)
            {
                ListSpecialized<EventHandler<SubscribedKeyPressEventArgs>> subscribers = null;
                if (mKeysSubscriptions.ContainsKey(subscription))
                {
                    subscribers = mKeysSubscriptions[subscription];
                }
                else
                {
                    subscribers = new ListSpecialized<EventHandler<SubscribedKeyPressEventArgs>>();
                    mKeysSubscriptions[subscription] = subscribers;
                }
                subscribers.Add(e);
            }
        }

        /// <summary>
        /// Unsubscribes for keys.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <param name="e">The decimal.</param>
        public void UnsubscribeForKeys(Keys keys, EventHandler<SubscribedKeyPressEventArgs> e)
        {
            if (e == null)
            {
                ThrowHelper.ThrowArgumentNullException("e");
            }

            SubscribeForKeys(new KeysSubscription(keys), e);
        }

        /// <summary>
        /// Unsubscribe for keys.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        /// <param name="e">The decimal.</param>
        public void UnsubscribeForKeys(KeysSubscription subscription, EventHandler<SubscribedKeyPressEventArgs> e)
        {
            if (subscription == null)
            {
                ThrowHelper.ThrowArgumentNullException("subscription");
            }
            if (e == null)
            {
                ThrowHelper.ThrowArgumentNullException("e");
            }

            lock (mKeysSubscriptions)
            {
                if (mKeysSubscriptions.ContainsKey(subscription))
                {
                    ListSpecialized<EventHandler<SubscribedKeyPressEventArgs>> list = mKeysSubscriptions[subscription];
                    IEnumeratorSpecialized<EventHandler<SubscribedKeyPressEventArgs>> en = list.GetEnumerator();
                    while (en.MoveNext())
                    {
                        if (en.Current.Method.Equals(e.Method) && en.Current.Target.Equals(e.Target))
                        {
                            en.Remove();
                            break;
                        }
                    }
                    if (list.Count == 0)
                    {
                        mKeysSubscriptions.Remove(subscription);
                    }
                }
            }
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Handles the operation system messages
        /// </summary>
        /// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message" /> to process.</param>
        protected override void WndProc(ref Message m)
        {
            if (this.IsHandleCreated)
            {
                if (m.Msg == (int)MouseInputNotificationEnum.WM_MOUSEWHEEL || m.Msg == (int)MouseInputNotificationEnum.WM_MOUSEHWHEEL)
                {
                    if (mProxy != null && mProxy.IsActive)
                    {
                        int xpos = LoWord(m.LParam);
                        int ypos = HiWord(m.LParam);
                        if (m.WParam.ToInt64() < int.MaxValue)
                        {
                            int mouseKeys = LoWord(m.WParam);
                            int delta = HiWord(m.WParam);
                            MouseButtons mb = MouseButtons.None;
                            switch (mouseKeys)
                            {
                                case MK_LBUTTON:
                                    mb = MouseButtons.Left;
                                    break;

                                case MK_RBUTTON:
                                    mb = MouseButtons.Right;
                                    break;

                                case MK_MBUTTON:
                                    mb = MouseButtons.Middle;
                                    break;
                            }

                            OnInternalMouseWheel(new MouseWheelInternalEventArgs(mb, 0, xpos, ypos, delta, (m.Msg == (int)MouseInputNotificationEnum.WM_MOUSEWHEEL) ? MouseWheelTypeEnum.Vertical : MouseWheelTypeEnum.Horizontal));
                        }
                    }
                }
                else if (VerticalScroll.Visible || HorizontalScroll.Visible)
                {
                    int oldValue = 0;
                    if (m.Msg == 0x20a)
                    {
                        oldValue = VerticalScroll.Value;
                    }
                    else if (m.Msg == 0x20e)
                    {
                        oldValue = HorizontalScroll.Value;
                    }

                    base.WndProc(ref m);

                    if (m.Msg == (int)MouseInputNotificationEnum.WM_MOUSEWHEEL && VerticalScroll.Visible)
                    {
                        OnScroll(new ScrollEventArgs((ScrollEventType)LoWord(m.WParam), oldValue, VerticalScroll.Value, ScrollOrientation.VerticalScroll));
                    }
                    else if (m.Msg == (int)MouseInputNotificationEnum.WM_MOUSEHWHEEL && HorizontalScroll.Visible)
                    {
                        OnScroll(new ScrollEventArgs((ScrollEventType)LoWord(m.WParam), oldValue, HorizontalScroll.Value, ScrollOrientation.HorizontalScroll));
                    }
                }
                else
                {
                    base.WndProc(ref m);
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        #endregion

        #region Internal method(s)

        internal void SendMouseMoveEvent(IRemoteDesktopClientInternal service, MouseMoveServiceEventArgs e)
        {
            Area area = mVisibleArea;

            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_CLIENT, service sent mouse move data. X: {0}, Y: {1}, CursorId: {2}. Current area, StartX: {3}, EndX: {4}, StartY: {5}, EndY: {6}", e.Position.X.ToString(), e.Position.Y.ToString(), e.CursorId, area.StartX.ToString(), area.EndX.ToString(), area.StartY.ToString(), area.EndY.ToString()));

            if (area.StartX <= e.Position.X && area.EndX >= e.Position.X &&
                area.StartY <= e.Position.Y && area.EndY >= e.Position.Y)
            {
                if (service.LastMousePosX != e.Position.X || service.LastMousePosY != e.Position.Y || !service.LastCursorId.Equals(e.CursorId))
                {
                    service.LastMousePosX = e.Position.X;
                    service.LastMousePosY = e.Position.Y;
                    service.LastCursorId = e.CursorId;

                    DrawMouse();
                }
                //else
                //{
                //    LOGGER.Debug("REMOTE_CLIENT, no mouse drawn #1.");
                //}
            }
            else
            {
                //LOGGER.Debug("REMOTE_CLIENT, no mouse drawn #2.");
                service.LastMousePosX = e.Position.X;
                service.LastMousePosY = e.Position.Y;
                service.LastCursorId = e.CursorId;
            }
        }

        internal void SendDesktopImageClip(IRemoteDesktopClientInternal service, DesktopImageClipArgs e)
        {
            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_CLIENT, service sent desktop clip refresh. X: {0}, Y: {1}. Size: {2}", e.Location.X.ToString(), e.Location.Y.ToString(), e.ImageData.Length.ToString()));
            DrawDesktopClip(e);
        }

        internal void SendClipboardContent(IRemoteDesktopClientInternal service, Forge.RemoteDesktop.Contracts.ClipboardChangedEventArgs e)
        {
            if (LOGGER.IsDebugEnabled) LOGGER.Debug(string.Format("REMOTE_CLIENT, service sent clipboard content. Text: {0}", e.Text));
            if (string.Compare(service.ClipboardContent, e.Text) != 0)
            {
                service.ClipboardContent = e.Text;
                SetClipboardContentFromClient(e.Text);
            }
        }

        #endregion

        #region Private method(s)

        private void DoDisposeCheck()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        private static int LoWord(IntPtr n)
        {
            return LoWord(n.ToInt32());
        }

        private static int LoWord(int n)
        {
            return (n & 0xffff);
        }

        private static int HiWord(IntPtr n)
        {
            return HiWord(n.ToInt32());
        }

        private static int HiWord(int n)
        {
            return (n / 0xffff);
        }

        private void SetClipboardContentFromClient(string text)
        {
            if (this.InvokeRequired)
            {
                SetClipboardContentHandler d = new SetClipboardContentHandler(SetClipboardContentFromClient);
                ((RemoteDesktopWinFormsControl)d.Target).Invoke(d, text);
                return;
            }

            Clipboard.SetText(text);
        }

        private void OnInternalMouseWheel(MouseWheelInternalEventArgs e)
        {
            if (this.IsDisposed)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                MouseWheelInternalHandler d = new MouseWheelInternalHandler(OnInternalMouseWheel);
                ((RemoteDesktopWinFormsControl)d.Target).Invoke(d, e);
                return;
            }

            if (this.Focused)
            {
                Point clientPoint = pbClient.PointToClient(e.Location);

                int x = 0;
                int y = 0;

                if (HorizontalScroll.Visible)
                {
                    x = HorizontalScroll.Value;
                }
                if (VerticalScroll.Visible)
                {
                    y = VerticalScroll.Value;
                }

                int endX = this.ClientRectangle.Width + x;
                int endY = this.ClientRectangle.Height + y;

                if (clientPoint.X >= x && clientPoint.X <= endX && clientPoint.Y >= y && clientPoint.Y <= endY)
                {
                    try
                    {
                        mProxy.ClientSendMouseWheelEvent(new MouseWheelEventArgs(e.WheelType, e.Delta));
                        //Console.WriteLine(string.Format("ClientPoint, X: {0}, Y: {1}", clientPoint.X.ToString(), clientPoint.Y.ToString()));
                    }
                    catch (Exception) { }
                }
            }
        }

        private void Proxy_Disconnected(object sender, DisconnectEventArgs e)
        {
            mProxy.Dispose();
            mMouseMoveWaitEvent.Set();
            UnsubscribeEvents();
            ClipboardEventManager.Instance.EventClipboardChanged -= new EventHandler<Native.Hooks.ClipboardChangedEventArgs>(ClipboardChangedEventHandler);
            DrawInitialBlackBackground();
            lock (mLockEventObject)
            {
                Raiser.CallDelegatorBySync(EventConnectionStateChange, new object[] { this, new ConnectionStateChangeEventArgs(false) });
            }
        }

        private void SubscribeEvents()
        {
            // picturebox events
            //this.pbClient.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbClient_MouseClick);
            //this.pbClient.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pbClient_MouseDoubleClick);
            this.pbClient.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbClient_MouseDown);
            this.pbClient.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbClient_MouseMove);
            this.pbClient.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbClient_MouseUp);

            // form events
            this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.RemoteDesktopWinFormsControl_Scroll);
            this.ClientSizeChanged += new System.EventHandler(this.RemoteDesktopWinFormsControl_ClientSizeChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Event_KeyDown);
            //this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Event_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Event_KeyUp);
        }

        private void UnsubscribeEvents()
        {
            // picturebox events
            //this.pbClient.MouseClick -= new System.Windows.Forms.MouseEventHandler(this.pbClient_MouseClick);
            //this.pbClient.MouseDoubleClick -= new System.Windows.Forms.MouseEventHandler(this.pbClient_MouseDoubleClick);
            this.pbClient.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.pbClient_MouseDown);
            this.pbClient.MouseMove -= new System.Windows.Forms.MouseEventHandler(this.pbClient_MouseMove);
            this.pbClient.MouseUp -= new System.Windows.Forms.MouseEventHandler(this.pbClient_MouseUp);

            // form events
            this.Scroll -= new System.Windows.Forms.ScrollEventHandler(this.RemoteDesktopWinFormsControl_Scroll);
            this.ClientSizeChanged -= new System.EventHandler(this.RemoteDesktopWinFormsControl_ClientSizeChanged);
            this.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.Event_KeyDown);
            //this.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.Event_KeyPress);
            this.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.Event_KeyUp);
        }

        private Area GetVisibleArea()
        {
            int x = 0;
            int y = 0;

            if (this.IsHandleCreated)
            {
                if (HorizontalScroll.Visible)
                {
                    x = HorizontalScroll.Value;
                }
                if (VerticalScroll.Visible)
                {
                    y = VerticalScroll.Value;
                }
            }

            return new Area(x, y, this.ClientRectangle.Width + x, this.ClientRectangle.Height + y);
        }

        private void ClipboardChangedEventHandler(object sender, Native.Hooks.ClipboardChangedEventArgs e)
        {
            IRemoteDesktopClientInternal proxy = mProxy;
            if (proxy != null && proxy.IsActive && e.ContainsText())
            {
                string text = e.GetText();
                if (string.Compare(text, proxy.ClipboardContent) != 0)
                {
                    try
                    {
                        proxy.ClientSendClipboardContent(new Contracts.ClipboardChangedEventArgs(text));
                    }
                    catch (Exception) { }
                    proxy.ClipboardContent = text;
                }
            }
        }

        private void Event_KeyDown(object sender, KeyEventArgs e)
        {
            bool skipKeyEvent = false;
            lock (mKeysSubscriptions)
            {
                skipKeyEvent = mKeysSubscriptions.ContainsKey(new KeysSubscription(e.KeyCode, e.Alt, e.Control, e.Shift));
            }

            if (!skipKeyEvent)
            {
                IRemoteDesktopClientInternal proxy = mProxy;
                if (proxy != null && proxy.IsActive)
                {
                    try
                    {
                        proxy.ClientSendKeyEvent(new KeyboardEventArgs(KeyboardEventTypeEnum.Down, e.KeyCode));
                    }
                    catch (Exception) { }
                }
                //LogKeyEvent(sender, "KEY-DOWN", e);
            }
        }

        private void Event_KeyUp(object sender, KeyEventArgs e)
        {
            ListSpecialized<EventHandler<SubscribedKeyPressEventArgs>> subscribeds = null;
            KeysSubscription k = new KeysSubscription(e.KeyCode, e.Alt, e.Control, e.Shift);
            lock (mKeysSubscriptions)
            {
                if (mKeysSubscriptions.ContainsKey(k))
                {
                    subscribeds = new ListSpecialized<EventHandler<SubscribedKeyPressEventArgs>>(mKeysSubscriptions[k]);
                }
            }

            if (subscribeds != null)
            {
                SubscribedKeyPressEventArgs arg = new SubscribedKeyPressEventArgs(k);
                foreach (EventHandler<SubscribedKeyPressEventArgs> handler in subscribeds)
                {
                    Raiser.CallDelegatorBySync(handler, new object[] { this, arg });
                }
            }
            else
            {
                IRemoteDesktopClientInternal proxy = mProxy;
                if (proxy != null && proxy.IsActive)
                {
                    try
                    {
                        proxy.ClientSendKeyEvent(new KeyboardEventArgs(KeyboardEventTypeEnum.Up, e.KeyCode));
                    }
                    catch (Exception) { }
                }
                //LogKeyEvent(sender, "KEY-UP", e);
            }
        }

        private void pbClient_MouseDown(object sender, MouseEventArgs e)
        {
            IRemoteDesktopClientInternal proxy = mProxy;
            if (proxy != null && proxy.IsActive)
            {
                try
                {
                    proxy.ClientSendMouseButtonEvent(new MouseButtonEventArgs(MouseButtonEventTypeEnum.Down, e.Button, e.Location));
                }
                catch (Exception) { }
            }
            //LogMouseEvent("PB-DOWN", e);
        }

        private void pbClient_MouseMove(object sender, MouseEventArgs e)
        {
            IRemoteDesktopClientInternal proxy = mProxy;
            if (proxy != null && proxy.IsActive)
            {
                proxy.LastMousePosX = e.Location.X;
                proxy.LastMousePosY = e.Location.Y;
                mClientMouseMove = true;
            }
            //LogMouseEvent("PB-MOVE", e);
        }

        private void pbClient_MouseUp(object sender, MouseEventArgs e)
        {
            IRemoteDesktopClientInternal proxy = mProxy;
            if (proxy != null && proxy.IsActive)
            {
                try
                {
                    proxy.ClientSendMouseButtonEvent(new MouseButtonEventArgs(MouseButtonEventTypeEnum.Up, e.Button, e.Location));
                }
                catch (Exception) { }
            }
            //LogMouseEvent("PB-UP", e);
        }

        private void pbClient_MouseEnter(object sender, EventArgs e)
        {
            // amikor belép a területre, át kell rajzolni a helyi Cursor-t
            IRemoteDesktopClientInternal proxy = mProxy;
            if (proxy != null && proxy.IsActive && !string.IsNullOrEmpty(proxy.LastCursorId))
            {
                Cursor.Current = mCursorWithIds[proxy.LastCursorId];
            }
        }

        private void RemoteDesktopWinFormsControl_Scroll(object sender, ScrollEventArgs e)
        {
            IRemoteDesktopClientInternal proxy = mProxy;
            if (proxy != null && proxy.IsActive)
            {
                mVisibleArea = GetVisibleArea();
                try
                {
                    proxy.ClientSubscribeForDesktop(mVisibleArea);
                }
                catch (Exception) { }
            }

            //Console.WriteLine(string.Format("SCROLL, Type: {0}, OldValue: {1}, NewValue: {2}, Orientation: {3}", e.Type.ToString(), e.OldValue.ToString(), e.NewValue.ToString(), e.ScrollOrientation.ToString()));
            //LogVisibleArea();
        }

        private void RemoteDesktopWinFormsControl_ClientSizeChanged(object sender, EventArgs e)
        {
            IRemoteDesktopClientInternal proxy = mProxy;
            if (proxy != null && proxy.IsActive)
            {
                mVisibleArea = GetVisibleArea();
                try
                {
                    proxy.ClientSubscribeForDesktop(mVisibleArea);
                }
                catch (Exception) { }
            }

            //Console.WriteLine(string.Format("FORM, new client size: {0}, {1}; Pos: {2}, {3}, HScroll: {4}, VScroll: {5}", this.ClientSize.Width.ToString(), this.ClientSize.Height.ToString(), this.AutoScrollPosition.X.ToString(), this.AutoScrollPosition.Y.ToString(), this.HorizontalScroll.Value.ToString(), this.VerticalScroll.Value.ToString()));
            //LogVisibleArea();
        }

        private void DrawInitialBlackBackground()
        {
            if (this.InvokeRequired)
            {
                Action d = new Action(DrawInitialBlackBackground);
                try
                {
                    ((RemoteDesktopWinFormsControl)d.Target).Invoke(d);
                }
                catch (Exception) { }
                return;
            }

            if (mServiceConfig != null)
            {
                mRemoteDesktopImage = new Bitmap(mServiceConfig.DesktopSize.Width, mServiceConfig.DesktopSize.Height);
                using (Graphics g = Graphics.FromImage(mRemoteDesktopImage))
                {
                    g.FillRectangle(Brushes.Black, 0, 0, mServiceConfig.DesktopSize.Width, mServiceConfig.DesktopSize.Height);
                }
                pbClient.Image = mRemoteDesktopImage;
            }
        }

        private void DrawMouse()
        {
            if (LOGGER.IsDebugEnabled) LOGGER.Debug("REMOTE_CLIENT, draw mouse to the surface.");

            if (this.InvokeRequired)
            {
                Action d = new Action(DrawMouse);
                ((RemoteDesktopWinFormsControl)d.Target).Invoke(d);
                return;
            }

            IRemoteDesktopClientInternal proxy = mProxy;
            Bitmap background = mRemoteDesktopImage.Clone() as Bitmap;
            if (proxy != null && !string.IsNullOrEmpty(proxy.LastCursorId))
            {
                using (Graphics g = Graphics.FromImage(background))
                {
                    Cursor c = mCursorWithIds[proxy.LastCursorId];
                    Rectangle cursorBounds = new Rectangle(new Point(proxy.LastMousePosX, proxy.LastMousePosY), c.Size);
                    c.Draw(g, cursorBounds);
                }
            }
            pbClient.Image = background;
        }

        private void DrawDesktopClip(DesktopImageClipArgs e)
        {
            if (this.InvokeRequired)
            {
                DrawDesktopClipHandler d = new DrawDesktopClipHandler(DrawDesktopClip);
                try
                {
                    ((RemoteDesktopWinFormsControl)d.Target).Invoke(d, e);
                }
                catch (Exception) { }
                return;
            }

            using (MemoryStream ms = new MemoryStream(e.ImageData))
            {
                ms.Position = 0;
                using (Bitmap clip = Bitmap.FromStream(ms) as Bitmap)
                {
                    using (Graphics g = Graphics.FromImage(mRemoteDesktopImage))
                    {
                        g.DrawImage(clip, e.Location);
                    }
                }
                ms.SetLength(0);
            }

            // egérnek látszódnia kell a felületen?
            IRemoteDesktopClientInternal proxy = mProxy;
            if (proxy != null &&
                proxy.LastMousePosX >= mVisibleArea.StartX && proxy.LastMousePosX <= mVisibleArea.EndX &&
                proxy.LastMousePosY >= mVisibleArea.StartY && proxy.LastMousePosY <= mVisibleArea.EndY)
            {
                // egér berajzolása a felületre
                DrawMouse();
            }
            else
            {
                pbClient.Image = mRemoteDesktopImage;
            }
        }

        private void MouseMoveMain()
        {
            if (LOGGER.IsDebugEnabled) LOGGER.Debug("REMOTE_CLIENT, mouse move sender thread started.");
            IRemoteDesktopClientInternal proxy = mProxy;
            if (proxy != null)
            {
                int lastX = proxy.LastMousePosX;
                int lastY = proxy.LastMousePosY;
                while (proxy.IsActive)
                {
                    try
                    {
                        mMouseMoveWaitEvent.WaitOne(Forge.RemoteDesktop.Client.Configuration.Settings.MouseMoveSendInterval);
                        if (mClientMouseMove)
                        {
                            if (lastX != proxy.LastMousePosX || lastY != proxy.LastMousePosY)
                            {
                                lastX = proxy.LastMousePosX;
                                lastY = proxy.LastMousePosY;
                                mClientMouseMove = false;
                                proxy.ClientSendMouseMoveEvent(new MouseMoveEventArgs(new Point(proxy.LastMousePosX, proxy.LastMousePosY)));
                                DrawMouse();
                            }
                        }
                    }
                    catch (Exception) { }
                }
            }
            if (LOGGER.IsDebugEnabled) LOGGER.Debug("REMOTE_CLIENT, mouse move sender thread stopped.");
        }

        #region Codes for debugging

        //private void LogVisibleArea()
        //{
        //    Area area = GetVisibleArea();
        //    StringBuilder sb = new StringBuilder("VISIBLE, ");
        //    sb.Append(", StartX: ");
        //    sb.Append(area.StartX.ToString());
        //    sb.Append(", StartY: ");
        //    sb.Append(area.StartY.ToString());
        //    sb.Append(", EndX: ");
        //    sb.Append(area.EndX.ToString());
        //    sb.Append(", EndY: ");
        //    sb.Append(area.EndY.ToString());
        //    //Console.WriteLine(sb.ToString());
        //}

        //private void LogMouseEvent(string eventName, MouseEventArgs e)
        //{
        //    StringBuilder sb = new StringBuilder(eventName);
        //    sb.Append(", Button: ");
        //    sb.Append(e.Button.ToString());
        //    sb.Append(", Clicks: ");
        //    sb.Append(e.Clicks);
        //    sb.Append(", Delta: ");
        //    sb.Append(e.Delta);
        //    sb.Append(", X: ");
        //    sb.Append(e.X.ToString());
        //    sb.Append(", Y: ");
        //    sb.Append(e.Y.ToString());
        //    //Console.WriteLine(sb.ToString());
        //}

        //private void LogKeyEvent(object sender, string eventName, KeyEventArgs e)
        //{
        //    StringBuilder sb = new StringBuilder(eventName);
        //    sb.Append(" (");
        //    sb.Append(sender == null ? "null" : sender.GetType().Name);
        //    sb.Append("), Alt: ");
        //    sb.Append(e.Alt.ToString());
        //    sb.Append(", Control: ");
        //    sb.Append(e.Control.ToString());
        //    sb.Append(", KeyCode: ");
        //    sb.Append(e.KeyCode.ToString());
        //    sb.Append(", KeyData: ");
        //    sb.Append(e.KeyData.ToString());
        //    sb.Append(", KeyValue: ");
        //    sb.Append(e.KeyValue.ToString());
        //    sb.Append(", Modifiers: ");
        //    sb.Append(e.Modifiers.ToString());
        //    sb.Append(", Shift: ");
        //    sb.Append(e.Shift.ToString());
        //    sb.Append(", SuppressKeyPress: ");
        //    sb.Append(e.SuppressKeyPress.ToString());
        //    //Console.WriteLine(sb.ToString());
        //}

        #endregion

        #endregion

        #region Nested type(s)

        internal class MouseWheelInternalEventArgs : MouseEventArgs
        {

            /// <summary>
            /// Initializes a new instance of the <see cref="MouseWheelInternalEventArgs"/> class.
            /// </summary>
            /// <param name="button">The button.</param>
            /// <param name="clicks">The clicks.</param>
            /// <param name="x">The executable.</param>
            /// <param name="y">The asynchronous.</param>
            /// <param name="delta">The delta.</param>
            /// <param name="wheelType">Type of the wheel.</param>
            internal MouseWheelInternalEventArgs(MouseButtons button, int clicks, int x, int y, int delta, MouseWheelTypeEnum wheelType)
                : base(button, clicks, x, y, delta)
            {
                this.WheelType = wheelType;
            }

            internal MouseWheelTypeEnum WheelType { get; private set; }

        }

        #endregion

    }

}
