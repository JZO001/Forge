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

    /// <summary>WebSocket handler</summary>
    public abstract class WebSocketHandler
    {

        /// <summary>Gets or sets the web socket connection manager.</summary>
        /// <value>The web socket connection manager.</value>
        protected WebSocketManager WebSocketConnectionManager { get; set; }

        /// <summary>Initializes a new instance of the <see cref="WebSocketHandler" /> class.</summary>
        /// <param name="webSocketConnectionManager">The web socket connection manager.</param>
        protected WebSocketHandler(WebSocketManager webSocketConnectionManager)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        /// <summary>Called when a socket connected.</summary>
        /// <param name="socket">The socket.</param>
        public virtual async Task OnConnected(WebSocket socket)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            WebSocketConnectionManager.AddSocket(socket);
        }

        /// <summary>Called when socket disconnected.</summary>
        /// <param name="socket">The socket.</param>
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
        /// <param name="skipThisWebSocketId">Skip the specified connection.</param>
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
        /// <param name="data">The message in byte array format.</param>
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
        /// <param name="data">The message is byte array.</param>
        /// <param name="skipThisWebSocketId">Skip the specified connection.</param>
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

            byte[] data = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(buffer: new ArraySegment<byte>(array: data, offset: 0, count: data.Length),
                                    messageType: WebSocketMessageType.Text,
                                    endOfMessage: isEndOfMessage,
                                    cancellationToken: CancellationToken.None);
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
