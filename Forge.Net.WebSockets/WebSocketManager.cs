/* *********************************************************************
 * Date: 10 Jun 2019
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Forge.Net.WebSockets
{

    /// <summary>WebSocket manager</summary>
    public class WebSocketManager
    {

        private readonly ConcurrentDictionary<string, WebSocket> mSockets = new ConcurrentDictionary<string, WebSocket>();

        /// <summary>Initializes a new instance of the <see cref="WebSocketManager" /> class.</summary>
        public WebSocketManager()
        {
        }

        /// <summary>Gets all registered sockets from the store</summary>
        /// <returns></returns>
        public ConcurrentDictionary<string, WebSocket> GetAllSockets()
        {
            return mSockets;
        }

        /// <summary>Gets the socket by identifier from the store</summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public WebSocket GetSocketById(string id)
        {
            return mSockets.FirstOrDefault(p => p.Key == id).Value;
        }

        /// <summary>Gets the identifier by socket.</summary>
        /// <param name="socket">The socket.</param>
        /// <returns></returns>
        public string GetIdBySocket(WebSocket socket)
        {
            return mSockets.FirstOrDefault(p => p.Value == socket).Key;
        }

        /// <summary>Adds a newly connected socket.</summary>
        /// <param name="socket">The socket.</param>
        public void AddSocket(WebSocket socket)
        {
            mSockets.TryAdd(CreateSocketId(), socket);
        }

        /// <summary>Removes a socket.</summary>
        /// <param name="id">The identifier.</param>
        public async Task RemoveSocket(string id)
        {
            WebSocket socket;
            mSockets.TryRemove(id, out socket);

            await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                    statusDescription: "Closed by the WebSocketManager",
                                    cancellationToken: CancellationToken.None);
        }

        private string CreateSocketId()
        {
            return Guid.NewGuid().ToString();
        }

    }

}
