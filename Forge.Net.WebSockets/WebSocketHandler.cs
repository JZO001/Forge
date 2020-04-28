/* *********************************************************************
 * Date: 10 Jun 2019
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Forge.Net.WebSockets
{

    public abstract class WebSocketHandler
    {

        protected WebSocketManager WebSocketConnectionManager { get; set; }

        protected WebSocketHandler(WebSocketManager webSocketConnectionManager)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public virtual async Task OnConnected(WebSocket socket)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            WebSocketConnectionManager.AddSocket(socket);
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetIdBySocket(socket));
        }

        /// <summary>Receives data the asynchronously, You need to implement your own handler to process the data.</summary>
        /// <param name="socket">The web socket instance</param>
        /// <param name="result">The receive result data</param>
        /// <param name="buffer">  Data, what is received in a bzte arraz</param>
        /// <returns></returns>
        public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);

        /// <summary>Broadcasts the message to all connected clients asynchronously.</summary>
        /// <param name="message">The message.</param>
        /// <param name="isEndOfMessage">  Set to <c>true at the end of the message</c></param>
        public virtual async Task BroadcastMessageToAllAsync(string message, bool isEndOfMessage = true)
        {
            foreach (var kvIdAndWS in WebSocketConnectionManager.GetAllSockets())
            {
                if (kvIdAndWS.Value.State == WebSocketState.Open)
                    await SendMessageAsync(kvIdAndWS.Value, message, isEndOfMessage);
            }
        }

        /// <summary>Broadcasts the message to all connected clients asynchronously, except the specified one This is useful, if you would like to skip a sender, which this message originally arrived.</summary>
        /// <param name="message">The message.</param>
        /// <param name="isEndOfMessage">  Set to <c>true at the end of the message</c></param>
        public virtual async Task BroadcastMessageToAllAsync(string message, string skipThisWebSocketId, bool isEndOfMessage = true)
        {
            foreach (var kvIdAndWS in WebSocketConnectionManager.GetAllSockets())
            {
                if (!kvIdAndWS.Key.Equals(skipThisWebSocketId) && kvIdAndWS.Value.State == WebSocketState.Open)
                    await SendMessageAsync(kvIdAndWS.Value, message, isEndOfMessage);
            }
        }

        /// <summary>Broadcasts the message to all connected clients asynchronously.</summary>
        /// <param name="message">The message in byte array format.</param>
        /// <param name="isEndOfMessage">  Set to <c>true at the end of the message</c></param>
        public virtual async Task BroadcastMessageToAllAsync(byte[] data, bool isEndOfMessage = true)
        {
            foreach (var kvIdAndWS in WebSocketConnectionManager.GetAllSockets())
            {
                if (kvIdAndWS.Value.State == WebSocketState.Open)
                    await SendMessageAsync(kvIdAndWS.Value, data, isEndOfMessage);
            }
        }

        /// <summary>Broadcasts the message to all connected clients asynchronously, except the specified one This is useful, if you would like to skip a sender, which this message originally arrived.</summary>
        /// <param name="message">The message is byte array.</param>
        /// <param name="isEndOfMessage">Set to <c>true at the end of the message</c></param>
        public virtual async Task BroadcastMessageToAllAsync(byte[] data, string skipThisWebSocketId, bool isEndOfMessage = true)
        {
            foreach (var kvIdAndWS in WebSocketConnectionManager.GetAllSockets())
            {
                if (!kvIdAndWS.Key.Equals(skipThisWebSocketId) && kvIdAndWS.Value.State == WebSocketState.Open)
                    await SendMessageAsync(kvIdAndWS.Value, data, isEndOfMessage);
            }
        }

        /// <summary>Sends the message to a specified client, identified by a socket id</summary>
        /// <param name="socketId">The socket identifier.</param>
        /// <param name="message">The message.</param>
        public async Task SendMessageAsync(string socketId, string message)
        {
            await SendMessageAsync(WebSocketConnectionManager.GetSocketById(socketId), message, true);
        }

        /// <summary>Sends the message to a specified client, identified by a socket instance</summary>
        /// <param name="socket">The socket.</param>
        /// <param name="message">The message.</param>
        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            await SendMessageAsync(socket, message, true);
        }

        /// <summary>Sends the message to a specified client, identified by a socket id</summary>
        /// <param name="socketId">The socket identifier.</param>
        /// <param name="data">The message in byte array.</param>
        public async Task SendMessageAsync(string socketId, byte[] data)
        {
            await SendMessageAsync(WebSocketConnectionManager.GetSocketById(socketId), data, true);
        }

        /// <summary>Sends the message to a specified client, identified by a socket instance</summary>
        /// <param name="socket">The socket.</param>
        /// <param name="data">The message in byte array.</param>
        public async Task SendMessageAsync(WebSocket socket, byte[] data)
        {
            await SendMessageAsync(socket, data, true);
        }

        /// <summary>Sends the message asynchronously</summary>
        /// <param name="socket">The socket.</param>
        /// <param name="message">The message.</param>
        /// <param name="isEndOfMessage">Set to <c>true at the end of the message</c></param>
        public virtual async Task SendMessageAsync(WebSocket socket, string message, bool isEndOfMessage)
        {
            if (socket.State != WebSocketState.Open) return;

            await SendMessageAsync(socket, Encoding.UTF8.GetBytes(message), isEndOfMessage);
        }

        /// <summary>Sends the message asynchronously</summary>
        /// <param name="socket">The socket.</param>
        /// <param name="data">The message in byte array.</param>
        /// <param name="isEndOfMessage">Set to <c>true at the end of the message</c></param>
        public virtual async Task SendMessageAsync(WebSocket socket, byte[] data, bool isEndOfMessage)
        {
            if (socket.State != WebSocketState.Open) return;

            await socket.SendAsync(buffer: new ArraySegment<byte>(data, offset: 0, count: data.Length),
                                    messageType: WebSocketMessageType.Binary,
                                    endOfMessage: isEndOfMessage,
                                    cancellationToken: CancellationToken.None);
        }

    }

}
