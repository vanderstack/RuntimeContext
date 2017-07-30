using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.DependencyModel;
using Vanderstack.RuntimeContext.Extensions;

namespace Vanderstack.RuntimeContext.Providers
{
    /// <summary>
    /// Provides the assemblies loaded at runtime.
    /// </summary>
    ///
    /// <remarks>
    /// Candidate assemblies are those which have been loaded into the set of
    /// runtime libaries by the .Net Core Framework.
    /// </remarks>
    public class RuntimeAssemblyProvider : IProvideAssemblies
    {
        public RuntimeAssemblyProvider()
        {
            _assemblies = InitializeAssemblies();
        }

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        public IEnumerable<Assembly> Assemblies =>
            _assemblies.Value;

        /// <summary>
        /// The assembly name prefix which all assemblies will have in common.
        /// </summary>
        protected virtual string AssemblyNamePrefix =>
            _assemblyNamePrefix;

        /// <summary>
        /// The assemblies
        /// </summary>
        ///
        /// <remarks>
        /// Derived from the assemblies loaded into the DependencyContext
        /// sharing the entry assembly name prefix.
        /// </remarks>
        private readonly Lazy<IEnumerable<Assembly>> _assemblies;

        /// <summary>
        /// The assembly name prefix which all assemblies will have in common.
        /// </summary>
        private readonly string _assemblyNamePrefix =
            Assembly.GetEntryAssembly().GetName().Name.Before(".").ToLower();

        /// <summary>
        /// Initializes the assemblies.
        /// </summary>
        private Lazy<IEnumerable<Assembly>> InitializeAssemblies()
        {
            return new Lazy<IEnumerable<Assembly>>(
                valueFactory:
                    () =>
                    DependencyContext
                    .Default
                    .RuntimeLibraries
                    .AsParallel()
                    .Where(runtimeLibrary =>
                        ContainsAssemblyNamePrefix(runtimeLibrary)
                        || HasDependencyContainingAssemblyNamePrefix(runtimeLibrary)
                    )
                    .Select(runtimeLibrary =>
                        new AssemblyName(runtimeLibrary.Name)
                    )
                    .Select(assemblyName =>
                        Assembly.Load(assemblyName)
                    )
                    // todo: implement lazy async throughout
                , mode: LazyThreadSafetyMode.ExecutionAndPublication
            );
        }

        /// <summary>
        /// Determines whether the specified runtime library contains the
        /// Assembly Name Prefix which identifies the assembly as having
        /// user defined types.
        /// </summary>
        private bool ContainsAssemblyNamePrefix(RuntimeLibrary runtimeLibrary)
        {
            return runtimeLibrary.Name.ToLower().Contains(AssemblyNamePrefix);
        }

        /// <summary>
        /// Determines whether the specified runtime library contains
        /// dependencies which contain the Assembly Name Prefix which
        /// identifies the assembly as having user defined types.
        /// </summary>
        private bool HasDependencyContainingAssemblyNamePrefix(RuntimeLibrary runtimeLibrary)
        {
            return runtimeLibrary.Dependencies.Any(dependency =>
                dependency.Name.ToLower().Contains(AssemblyNamePrefix)
            );
        }
    }
}
