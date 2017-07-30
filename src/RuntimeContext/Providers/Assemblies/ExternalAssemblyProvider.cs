using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;

namespace Vanderstack.RuntimeContext.Providers
{
    /// <summary>
    /// Provides the assemblies for the project.
    /// </summary>
    ///
    /// <remarks>
    /// Candidate assemblies are those which exist within the project directory.
    /// </remarks>
    public class ExternalAssemblyProvider : IProvideAssemblies
    {
        public ExternalAssemblyProvider(IProvidePaths pathProvider)
        {
            _pathProvider = pathProvider;

            _assemblies = InitializeAssemblies();
        }

        /// <summary>
        /// The path provider
        /// </summary>
        private readonly IProvidePaths _pathProvider;

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        public IEnumerable<Assembly> Assemblies => _assemblies.Value;

        /// <summary>
        /// The assemblies
        /// </summary>
        private readonly Lazy<IEnumerable<Assembly>> _assemblies;

        /// <summary>
        /// Initializes the assemblies.
        /// </summary>
        private Lazy<IEnumerable<Assembly>> InitializeAssemblies()
        {
            return new Lazy<IEnumerable<Assembly>>(
                valueFactory: () =>
                {
                    return
                        _pathProvider
                        .Assemblies
                        .AsParallel()
                        .Select(assemblyPath =>
                            AssemblyLoadContext
                            .Default
                            .LoadFromAssemblyPath(assemblyPath)
                        );
                }
                , mode: LazyThreadSafetyMode.ExecutionAndPublication
            );
        }
    }
}
