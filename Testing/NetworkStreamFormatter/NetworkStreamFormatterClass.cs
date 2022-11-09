using System.Runtime.InteropServices;

namespace Forge.Testing.NetworkStreamFormatter
{
    [ComVisible(true)]
    public sealed class NetworkStreamFormatterClass
    {
        //public NetworkStreamFormatterClass();
        //// Summary:
        ////     Gets or sets the behavior of the deserializer with regards to finding and
        ////     loading assemblies.
        ////
        //// Returns:
        ////     One of the System.Runtime.Serialization.Formatters.FormatterAssemblyStyle
        ////     values that specifies the deserializer behavior.
        //public FormatterAssemblyStyle AssemblyFormat { get; set; }
        ////
        //// Summary:
        ////     Gets or sets an object of type System.Runtime.Serialization.SerializationBinder
        ////     that controls the binding of a serialized object to a type.
        ////
        //// Returns:
        ////     The serialization binder to use with this formatter.
        //public SerializationBinder Binder { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the System.Runtime.Serialization.StreamingContext for this formatter.
        ////
        //// Returns:
        ////     The streaming context to use with this formatter.
        //public StreamingContext Context { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the System.Runtime.Serialization.Formatters.TypeFilterLevel
        ////     of automatic deserialization the System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
        ////     performs.
        ////
        //// Returns:
        ////     The System.Runtime.Serialization.Formatters.TypeFilterLevel that represents
        ////     the current automatic deserialization level.
        //public TypeFilterLevel FilterLevel { get; set; }
        ////
        //// Summary:
        ////     Gets or sets a System.Runtime.Serialization.ISurrogateSelector that controls
        ////     type substitution during serialization and deserialization.
        ////
        //// Returns:
        ////     The surrogate selector to use with this formatter.
        //public ISurrogateSelector SurrogateSelector { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the format in which type descriptions are laid out in the serialized
        ////     stream.
        ////
        //// Returns:
        ////     The style of type layouts to use.
        //public FormatterTypeStyle TypeFormat { get; set; }

        //// Summary:
        ////     Deserializes the specified stream into an object graph.
        ////
        //// Parameters:
        ////   serializationStream:
        ////     The stream from which to deserialize the object graph.
        ////
        //// Returns:
        ////     The top (root) of the object graph.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The serializationStream is null.
        ////
        ////   System.Runtime.Serialization.SerializationException:
        ////     The serializationStream supports seeking, but its length is 0.
        ////
        ////   System.Security.SecurityException:
        ////     The caller does not have the required permission.
        //public object Deserialize(Stream serializationStream);
        ////
        //// Summary:
        ////     Deserializes the specified stream into an object graph. The provided System.Runtime.Remoting.Messaging.HeaderHandler
        ////     handles any headers in that stream.
        ////
        //// Parameters:
        ////   serializationStream:
        ////     The stream from which to deserialize the object graph.
        ////
        ////   handler:
        ////     The System.Runtime.Remoting.Messaging.HeaderHandler that handles any headers
        ////     in the serializationStream. Can be null.
        ////
        //// Returns:
        ////     The deserialized object or the top object (root) of the object graph.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The serializationStream is null.
        ////
        ////   System.Runtime.Serialization.SerializationException:
        ////     The serializationStream supports seeking, but its length is 0.
        ////
        ////   System.Security.SecurityException:
        ////     The caller does not have the required permission.
        //public object Deserialize(Stream serializationStream, HeaderHandler handler);
        ////
        //// Summary:
        ////     Deserializes a response to a remote method call from the provided System.IO.Stream.
        ////
        //// Parameters:
        ////   serializationStream:
        ////     The stream from which to deserialize the object graph.
        ////
        ////   handler:
        ////     The System.Runtime.Remoting.Messaging.HeaderHandler that handles any headers
        ////     in the serializationStream. Can be null.
        ////
        ////   methodCallMessage:
        ////     The System.Runtime.Remoting.Messaging.IMethodCallMessage that contains details
        ////     about where the call came from.
        ////
        //// Returns:
        ////     The deserialized response to the remote method call.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The serializationStream is null.
        ////
        ////   System.Runtime.Serialization.SerializationException:
        ////     The serializationStream supports seeking, but its length is 0.
        ////
        ////   System.Security.SecurityException:
        ////     The caller does not have the required permission.
        //public object DeserializeMethodResponse(Stream serializationStream, HeaderHandler handler, IMethodCallMessage methodCallMessage);
        ////
        //// Summary:
        ////     Serializes the object, or graph of objects with the specified top (root),
        ////     to the given stream.
        ////
        //// Parameters:
        ////   serializationStream:
        ////     The stream to which the graph is to be serialized.
        ////
        ////   graph:
        ////     The object at the root of the graph to serialize.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The serializationStream is null. -or- The graph is null.
        ////
        ////   System.Runtime.Serialization.SerializationException:
        ////     An error has occurred during serialization, such as if an object in the graph
        ////     parameter is not marked as serializable.
        ////
        ////   System.Security.SecurityException:
        ////     The caller does not have the required permission.
        //public void Serialize(Stream serializationStream, object graph);
        ////
        //// Summary:
        ////     Serializes the object, or graph of objects with the specified top (root),
        ////     to the given stream attaching the provided headers.
        ////
        //// Parameters:
        ////   serializationStream:
        ////     The stream to which the object is to be serialized.
        ////
        ////   graph:
        ////     The object at the root of the graph to serialize.
        ////
        ////   headers:
        ////     Remoting headers to include in the serialization. Can be null.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The serializationStream is null.
        ////
        ////   System.Runtime.Serialization.SerializationException:
        ////     An error has occurred during serialization, such as if an object in the graph
        ////     parameter is not marked as serializable.
        ////
        ////   System.Security.SecurityException:
        ////     The caller does not have the required permission.
        //public void Serialize(Stream serializationStream, object graph, Header[] headers);
        ////
        //// Summary:
        ////     Deserializes the specified stream into an object graph. The provided System.Runtime.Remoting.Messaging.HeaderHandler
        ////     handles any headers in that stream.
        ////
        //// Parameters:
        ////   serializationStream:
        ////     The stream from which to deserialize the object graph.
        ////
        ////   handler:
        ////     The System.Runtime.Remoting.Messaging.HeaderHandler that handles any headers
        ////     in the serializationStream. Can be null.
        ////
        //// Returns:
        ////     The deserialized object or the top object (root) of the object graph.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The serializationStream is null.
        ////
        ////   System.Runtime.Serialization.SerializationException:
        ////     The serializationStream supports seeking, but its length is 0.
        ////
        ////   System.Security.SecurityException:
        ////     The caller does not have the required permission.
        //[ComVisible(false)]
        //public object UnsafeDeserialize(Stream serializationStream, HeaderHandler handler);
        ////
        //// Summary:
        ////     Deserializes a response to a remote method call from the provided System.IO.Stream.
        ////
        //// Parameters:
        ////   serializationStream:
        ////     The stream from which to deserialize the object graph.
        ////
        ////   handler:
        ////     The System.Runtime.Remoting.Messaging.HeaderHandler that handles any headers
        ////     in the serializationStream. Can be null.
        ////
        ////   methodCallMessage:
        ////     The System.Runtime.Remoting.Messaging.IMethodCallMessage that contains details
        ////     about where the call came from.
        ////
        //// Returns:
        ////     The deserialized response to the remote method call.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The serializationStream is null.
        ////
        ////   System.Runtime.Serialization.SerializationException:
        ////     The serializationStream supports seeking, but its length is 0.
        ////
        ////   System.Security.SecurityException:
        ////     The caller does not have the required permission.
        //[ComVisible(false)]
        //public object UnsafeDeserializeMethodResponse(Stream serializationStream, HeaderHandler handler, IMethodCallMessage methodCallMessage);
    
    }
}
