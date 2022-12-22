/* *********************************************************************
 * Date: 06 Aug 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Net.Remoting;

namespace Forge.RemoteDesktop.Contracts
{

    /// <summary>
    /// Represents the service methods which are used to communicate with the client
    /// </summary>
    [ServiceContract(WellKnownObjectMode = WellKnownObjectModeEnum.PerSession)]
    public interface IRemoteDesktopService : IRemoteDesktopPeer
    {

        /// <summary>
        /// Sends mouse move event to the client.
        /// </summary>
        /// <param name="args">The <see cref="MouseMoveServiceEventArgs"/> instance containing the event data.</param>
        [OperationContract(IsOneWay = true, IsReliable = false, Direction = OperationDirectionEnum.ClientSide)]
        void ServiceSendMouseMoveEvent(MouseMoveServiceEventArgs args);

        /// <summary>
        /// Sends desktop image clip to the client.
        /// </summary>
        /// <param name="args">The <see cref="DesktopImageClipArgs"/> instance containing the event data.</param>
        [OperationContract(IsOneWay = true, IsReliable = false, Direction = OperationDirectionEnum.ClientSide)]
        void ServiceSendDesktopImageClip(DesktopImageClipArgs args);

        /// <summary>
        /// Send the clipboard content to the client.
        /// </summary>
        /// <param name="args">The <see cref="ClipboardChangedEventArgs"/> instance containing the event data.</param>
        [OperationContract(IsOneWay = true, IsReliable = false, Direction = OperationDirectionEnum.ClientSide)]
        void ServiceSendClipboardContent(ClipboardChangedEventArgs args);

    }

}
