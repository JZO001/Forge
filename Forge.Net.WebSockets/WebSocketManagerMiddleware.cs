/* *********************************************************************
 * Date: 10 Jun 2019
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

#if NETSTANDARD2_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER

using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Forge.Net.WebSockets
{

    /// <summary>MiddleWare class for WebSocket Management</summary>
    public class WebSocketManagerMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly WebSocketHandler _webSocketHandler;

        /// <summary>Initializes a new instance of the <see cref="WebSocketManagerMiddleware"/> class.</summary>
        /// <param name="next">The next.</param>
        /// <param name="webSocketHandler">The web socket handler.</param>
        public WebSocketManagerMiddleware(RequestDelegate next, WebSocketHandler webSocketHandler)
        {
            _next = next;
            _webSocketHandler = webSocketHandler;
        }

        /// <summary>Gets or sets the size of the receive buffer.</summary>
        /// <value>The size of the receive buffer.</value>
        public static int ReceiveBufferSize { get; set; } = 4096;

        /// <summary>
        ///   <para>
        ///  Invoke for the web application. Here receives the new connections and start listening for incoming data.</para>
        /// </summary>
        /// <param name="context">The context.</param>
        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest) return;

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            await _webSocketHandler.OnConnected(socket);

            await Receive(socket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocketHandler.OnDisconnected(socket);
                    return;
                }
                else
                {
                    await _webSocketHandler.ReceiveAsync(socket, result, buffer);
                    return;
                }
            });
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            byte[] buffer = new byte[ReceiveBufferSize];
            while (socket.State == WebSocketState.Open)
            {
                if (buffer.Length != ReceiveBufferSize) buffer = new byte[ReceiveBufferSize];
                handleMessage(
                    await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), cancellationToken: CancellationToken.None),
                    buffer
                );
            }
        }

    }

}

#endif
