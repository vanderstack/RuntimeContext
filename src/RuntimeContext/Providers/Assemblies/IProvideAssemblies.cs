using System.Collections.Generic;
using System.Reflection;

namespace Vanderstack.RuntimeContext.Providers
{
    public interface IProvideAssemblies
    {
        IEnumerable<Assembly> Assemblies { get; }
    }
}
