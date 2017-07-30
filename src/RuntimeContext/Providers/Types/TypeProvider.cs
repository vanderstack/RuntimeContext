using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Vanderstack.RuntimeContext.Providers
{
    public class TypeProvider : IProvideTypes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeProvider"/> class.
        /// </summary>
        /// <param name="assemblyManager">The assembly manager.</param>
        public TypeProvider(IAssemblyManager assemblyManager)
        {
            _assemblyManager = assemblyManager;

            _types = InitializeTypes();
        }

        /// <summary>
        /// The assembly manager
        /// </summary>
        private readonly IAssemblyManager _assemblyManager;

        /// <summary>
        /// Gets the types.
        /// </summary>
        public IEnumerable<Type> Types =>
            _types.Value;

        /// <summary>
        /// The types
        /// </summary>
        private readonly Lazy<IEnumerable<Type>> _types;

        /// <summary>
        /// Initializes the types.
        /// </summary>
        private Lazy<IEnumerable<Type>> InitializeTypes()
        {
            return new Lazy<IEnumerable<Type>>(
                valueFactory: () =>
                {
                    return _assemblyManager
                        .Assemblies
                        .AsParallel()
                        .SelectMany(assembly =>
                            assembly.ExportedTypes
                        )
                        .ToArray();
                }
                , mode: LazyThreadSafetyMode.ExecutionAndPublication
            );
        }
    }
}
