using System;
using System.Reflection;

namespace Forge.Logging
{

    /// <summary>Wrapper interface for logger implementation</summary>
    public interface ILoggerWrapper
    {

#if NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
#else
        ILog GetLogger(string name);
#endif

        ILog GetLogger(string repository, string name);

        ILog GetLogger(Assembly repositoryAssembly, Type type);

        ILog GetLogger(string repository, Type type);

        ILog GetLogger(Type type);

        ILog GetLogger(Assembly repositoryAssembly, string name);

    }

}
