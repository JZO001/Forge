using Forge.Collections;
using Forge.Configuration;
using Forge.Net.Remoting.Messaging;
using Forge.Net.Remoting.Sinks;
using Forge.Net.Synapse;
using Forge.Threading.Tasking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forge.Net.Remoting.Channels
{

    /// <summary>Represents a channel base members</summary>
    public interface IChannel : IInitializable, IDisposable
    {

        /// <summary>Occurs when [session state change].</summary>
        event EventHandler<SessionStateEventArgs> SessionStateChange;

        /// <summary>Occurs when [receive message].</summary>
        event EventHandler<ReceiveMessageEventArgs> ReceiveMessage;

        /// <summary>Gets the channel identifier.</summary>
        /// <value>The channel identifier.</value>
        string ChannelId { get; }

        /// <summary>Gets a value indicating whether this instance is stream supported.</summary>
        /// <value>
        ///   <c>true</c> if this instance is stream supported; otherwise, <c>false</c>.</value>
        bool IsStreamSupported { get; }

        /// <summary>Gets a value indicating whether this instance is disposed.</summary>
        /// <value>
        ///   <c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
        bool IsDisposed { get; }

        /// <summary>Gets the default connection data.</summary>
        /// <value>The default connection data.</value>
        AddressEndPoint DefaultConnectionData { get; }

        /// <summary>Gets or sets the default error response timeout.</summary>
        /// <value>The default error response timeout.</value>
        long DefaultErrorResponseTimeout { get; set; }

        /// <summary>Gets the server endpoints.</summary>
        /// <value>The server endpoints.</value>
        ICollection<AddressEndPoint> ServerEndpoints { get; }

        /// <summary>Gets a value indicating whether this instance is session reusable.</summary>
        /// <value>
        ///   <c>true</c> if this instance is session reusable; otherwise, <c>false</c>.</value>
        bool IsSessionReusable { get; }

#if NET40
        /// <summary>Begins the connect.</summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        IAsyncResult BeginConnect(AsyncCallback callback, object state);

        /// <summary>Begins the connect to.</summary>
        /// <param name="remoteEp">The remote ep.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        IAsyncResult BeginConnectTo(AddressEndPoint remoteEp, AsyncCallback callback, object state);

        /// <summary>Ends the connect.</summary>
        /// <param name="asyncResult">The asynchronous result.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        string EndConnect(IAsyncResult asyncResult);

        /// <summary>Ends the connect to.</summary>
        /// <param name="asyncResult">The asynchronous result.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        string EndConnectTo(IAsyncResult asyncResult);
#endif

        /// <summary>Begins the connect.</summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        ITaskResult BeginConnect(ReturnCallback callback, object state);

        /// <summary>Begins the connect to.</summary>
        /// <param name="remoteEp">The remote ep.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        ITaskResult BeginConnectTo(AddressEndPoint remoteEp, ReturnCallback callback, object state);

        /// <summary>Ends the connect.</summary>
        /// <param name="asyncResult">The asynchronous result.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        string EndConnect(ITaskResult asyncResult);

        /// <summary>Ends the connect to.</summary>
        /// <param name="asyncResult">The asynchronous result.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        string EndConnectTo(ITaskResult asyncResult);

        /// <summary>Connects this instance.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        string Connect();

        /// <summary>Connects to.</summary>
        /// <param name="remoteEp">The remote ep.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        string ConnectTo(AddressEndPoint remoteEp);

        /// <summary>Disconnects the specified session identifier.</summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        bool Disconnect(string sessionId);

 #if NETCOREAPP3_1_OR_GREATER
        /// <summary>Sends the message asynchronous.</summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        Task<IMessage> SendMessageAsync(string sessionId, IMessage message);

        /// <summary>Sends the message asynchronous.</summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="message">The message.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        Task<IMessage> SendMessageAsync(string sessionId, IMessage message, long timeout);
#endif

        /// <summary>Sends the message.</summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        IMessage SendMessage(string sessionId, IMessage message);

        /// <summary>Sends the message.</summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="message">The message.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        IMessage SendMessage(string sessionId, IMessage message, long timeout);

        /// <summary>Starts the listening.</summary>
        void StartListening();

        /// <summary>Stops the listening.</summary>
        void StopListening();

        /// <summary>Gets the send message sinks.</summary>
        /// <value>The send message sinks.</value>
        IEnumerableSpecialized<IMessageSink> SendMessageSinks { get; }

        /// <summary>Gets the receive message sinks.</summary>
        /// <value>The receive message sinks.</value>
        IEnumerableSpecialized<IMessageSink> ReceiveMessageSinks { get; }

        /// <summary>Gets the session information.</summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        ISessionInfo GetSessionInfo(string sessionId);

        /// <summary>Determines whether the specified session identifier is connected.</summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>
        ///   <c>true</c> if the specified session identifier is connected; otherwise, <c>false</c>.</returns>
        bool IsConnected(string sessionId);

    }

}
