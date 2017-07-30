using System;
using System.Threading;

namespace Vanderstack.RuntimeContext
{
    // todo: further extend with lazy async
    // https://github.com/StephenCleary/AsyncEx

    public class RuntimeContext : IRuntimeContext
    {
        public RuntimeContext()
        {
            _pathManager = InitializePathManager();
            _assemblyManager = InitializeAssemblyManager();
            _typeManager = InitializeTypeManager();
            _environmentSettings = InitializeEnvironmentSettings();
            _applicationSettings = InitializeApplicationSettings();
        }

        private string _overrideEnvironmentName;

        public IPathManager PathManager =>
            _pathManager.Value;

        private readonly Lazy<IPathManager> _pathManager;

        public IAssemblyManager AssemblyManager =>
            _assemblyManager.Value;

        private readonly Lazy<IAssemblyManager> _assemblyManager;

        public ITypeManager TypeManager =>
            _typeManager.Value;

        private readonly Lazy<ITypeManager> _typeManager;

        public IEnvironmentSettings EnvironmentSettings =>
            _environmentSettings.Value;

        private readonly Lazy<IEnvironmentSettings> _environmentSettings;

        public IApplicationSettings ApplicationSettings =>
            _applicationSettings.Value;

        private readonly Lazy<IApplicationSettings> _applicationSettings;

        public void UseEnvironment(string environmentName)
        {
            if (_environmentSettings.IsValueCreated)
            {
                // todo: custom exceptions throughout for polish.
                throw new Exception($"The arguement {nameof(environmentName)} which will override the value from the environment settings file must be provided prior to any properties being accessed on the {nameof(RuntimeContext)}.");
            }

            _overrideEnvironmentName = environmentName;
        }

        protected virtual Lazy<IPathManager> InitializePathManager()
        {
            return new Lazy<IPathManager>(
                valueFactory: () =>
                {
                    return new PathManager();
                }
                , mode: LazyThreadSafetyMode.ExecutionAndPublication
            );
        }

        protected virtual Lazy<IAssemblyManager> InitializeAssemblyManager()
        {
            return new Lazy<IAssemblyManager>(
                valueFactory: () =>
                {
                    return new AssemblyManager(PathManager);
                }
                , mode: LazyThreadSafetyMode.ExecutionAndPublication
            );
        }

        protected virtual Lazy<ITypeManager> InitializeTypeManager()
        {
            return new Lazy<ITypeManager>(
                valueFactory: () =>
                {
                    return new TypeManager(AssemblyManager);
                }
                , mode: LazyThreadSafetyMode.ExecutionAndPublication
            );
        }

        protected virtual Lazy<IEnvironmentSettings> InitializeEnvironmentSettings()
        {
            return new Lazy<IEnvironmentSettings>(
                valueFactory: () =>
                {
                    return new EnvironmentSettings(PathManager, _overrideEnvironmentName);
                }
                , mode: LazyThreadSafetyMode.ExecutionAndPublication
            );
        }

        protected virtual Lazy<IApplicationSettings> InitializeApplicationSettings()
        {
            return new Lazy<IApplicationSettings>(
                valueFactory: () =>
                {
                    return new ApplicationSettings(
                        PathManager
                        , AssemblyManager
                        , TypeManager
                        , EnvironmentSettings
                    );
                }
                , mode: LazyThreadSafetyMode.ExecutionAndPublication
            );
        }
    }
}
