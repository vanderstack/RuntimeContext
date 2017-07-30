using System;
using System.Collections.Generic;
using Vanderstack.RuntimeContext.Providers;

namespace Vanderstack.RuntimeContext
{
    public class TypeManager : ITypeManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeManager"/> class.
        /// </summary>
        /// <param name="assemblyManager">The assembly manager.</param>
        public TypeManager(IAssemblyManager assemblyManager)
            : this(new TypeProvider(assemblyManager))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeManager"/> class.
        /// </summary>
        /// <param name="typeProvider">The type provider.</param>
        protected TypeManager(IProvideTypes typeProvider)
        {
            _typeProvider = typeProvider;
        }

        /// <summary>
        /// Gets the types.
        /// </summary>
        public IEnumerable<Type> Types =>
            _typeProvider.Types;

        /// <summary>
        /// The type provider
        /// </summary>
        private readonly IProvideTypes _typeProvider;
    }
}
