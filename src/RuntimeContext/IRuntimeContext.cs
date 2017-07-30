namespace Vanderstack.RuntimeContext
{
    public interface IRuntimeContext
    {
        IPathManager PathManager { get; }

        IAssemblyManager AssemblyManager { get; }

        ITypeManager TypeManager { get; }

        IEnvironmentSettings EnvironmentSettings { get; }

        IApplicationSettings ApplicationSettings { get; }
    }
}