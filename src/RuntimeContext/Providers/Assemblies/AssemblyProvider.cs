using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Vanderstack.RuntimeContext.Extensions;

namespace Vanderstack.RuntimeContext.Providers
{
    public class AssemblyProvider : IProvideAssemblies
    {
        public AssemblyProvider(IPathManager pathManager)
        {
            _pathManager = pathManager;
            _assemblyProviders = InitializeAssemblyProviders();
        }

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        public IEnumerable<Assembly> Assemblies =>
            _assemblyProviders.Value.SelectMany(assemblyProvider =>
                assemblyProvider.Assemblies
            );

        /// <summary>
        /// The path manager
        /// </summary>
        private readonly IPathManager _pathManager;

        /// <summary>
        /// The assembly providers
        /// </summary>
        private readonly Lazy<IEnumerable<IProvideAssemblies>> _assemblyProviders;

        /// <summary>
        /// Initializes the assembly providers.
        /// </summary>
        private Lazy<IEnumerable<IProvideAssemblies>> InitializeAssemblyProviders()
        {
            return new Lazy<IEnumerable<IProvideAssemblies>>(
                valueFactory: () =>
                {
                    var runtimeAssemblyProvider =
                        new RuntimeAssemblyProvider()
                        .AsEnumerable<IProvideAssemblies>();

                    var externalAssemblyProvider =
                        new ExternalAssemblyProvider(
                            _pathManager
                        )
                        .AsEnumerable<IProvideAssemblies>();

                    return
                        runtimeAssemblyProvider
                        .Concat(externalAssemblyProvider)
                        .Distinct()
                        .ToArray();
                }
                , mode: LazyThreadSafetyMode.ExecutionAndPublication
            );
        }
    }
}
