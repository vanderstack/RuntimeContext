using System.Collections.Generic;
using System.Reflection;
using Vanderstack.RuntimeContext.Providers;

namespace Vanderstack.RuntimeContext
{
    public class AssemblyManager : IAssemblyManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyManager"/> class.
        /// </summary>
        public AssemblyManager(IPathManager pathManager)
            : this(new AssemblyProvider(pathManager))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyManager"/> class.
        /// </summary>
        /// <param name="assemblyProvider">The assembly provider.</param>
        protected AssemblyManager(IProvideAssemblies assemblyProvider)
        {
            _assemblyProvider = assemblyProvider;
        }

        /// <summary>
        /// The assembly provider
        /// </summary>
        private readonly IProvideAssemblies _assemblyProvider;

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        public IEnumerable<Assembly> Assemblies =>
            _assemblyProvider.Assemblies;
    }
}
