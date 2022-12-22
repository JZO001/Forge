/* *********************************************************************
 * Date: 06 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.IO;
using Forge.Net.Remoting;

namespace Forge.RemoteDesktop.Contracts
{

    /// <summary>
    /// Represents the client side methods
    /// </summary>
    [ServiceContract(WellKnownObjectMode = WellKnownObjectModeEnum.PerSession)]
    public interface IRemoteDesktopClient : IRemoteDesktopPeer
    {

        /// <summary>
        /// Gets the authentication information.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        AuthModeResponseArgs GetAuthenticationInfo();

        /// <summary>
        /// Login into the remote service
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [OperationContract]
        LoginResponseArgs Login(LoginRequestArgs request);

        /// <summary>
        /// Gets the configuration of the service.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        DescriptionResponseArgs ClientGetConfiguration();

        /// <summary>
        /// Sets image clip quality.
        /// </summary>
        /// <param name="args">The args.</param>
        [OperationContract]
        void ClientSetImageClipQuality(ImageClipQualityArgs args);

        /// <summary>
        /// Subscribe for desktop client area to receive image refresh messages.
        /// </summary>
        /// <param name="area">The area.</param>
        [OperationContract(IsOneWay = true, IsReliable = true)]
        void ClientSubscribeForDesktop(Area area);

        /// <summary>
        /// Start the event pump.
        /// </summary>
        [OperationContract(IsOneWay = false, IsReliable = true)]
        MouseMoveServiceEventArgs ClientStartEventPump();

        /// <summary>
        /// Stops the event pump.
        /// </summary>
        [OperationContract(IsOneWay = false, IsReliable = true)]
        void ClientStopEventPump();

        /// <summary>
        /// Requests a desktop refresh from the service.
        /// </summary>
        [OperationContract(IsOneWay = true, IsReliable = false)]
        void ClientRefreshDesktop();

        /// <summary>
        /// Send the key event.
        /// </summary>
        /// <param name="args">The <see cref="KeyboardEventArgs"/> instance containing the event data.</param>
        [OperationContract(IsOneWay = true, IsReliable = false)]
        void ClientSendKeyEvent(KeyboardEventArgs args);

        /// <summary>
        /// Send mouse button event.
        /// </summary>
        /// <param name="args">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        [OperationContract(IsOneWay = true, IsReliable = false)]
        void ClientSendMouseButtonEvent(MouseButtonEventArgs args);

        /// <summary>
        /// Send mouse wheel event.
        /// </summary>
        /// <param name="args">The <see cref="MouseWheelEventArgs"/> instance containing the event data.</param>
        [OperationContract(IsOneWay = true, IsReliable = false)]
        void ClientSendMouseWheelEvent(MouseWheelEventArgs args);

        /// <summary>
        /// Sends mouse move event.
        /// </summary>
        /// <param name="args">The <see cref="MouseMoveEventArgs"/> instance containing the event data.</param>
        [OperationContract(IsOneWay = true, IsReliable = false)]
        void ClientSendMouseMoveEvent(MouseMoveEventArgs args);

        /// <summary>
        /// Sends the clipboard content to the service
        /// </summary>
        /// <param name="args">The <see cref="ClipboardChangedEventArgs"/> instance containing the event data.</param>
        [OperationContract(IsOneWay = true, IsReliable = false)]
        void ClientSendClipboardContent(ClipboardChangedEventArgs args);

        /// <summary>
        /// Sends a file to the service.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="file">The file.</param>
        [OperationContract(IsOneWay = true, IsReliable = true, CallTimeout = int.MaxValue, AllowParallelExecution = true)]
        void ClientSendFile(string fileName, Stream file);

    }

}
