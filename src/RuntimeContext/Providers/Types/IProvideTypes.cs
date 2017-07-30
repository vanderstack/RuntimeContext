using System;
using System.Collections.Generic;

namespace Vanderstack.RuntimeContext.Providers
{
    public interface IProvideTypes
    {
        /// <summary>
        /// Gets the types.
        /// </summary>
        IEnumerable<Type> Types { get; }
    }
}
