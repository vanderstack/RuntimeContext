using System;

namespace Vanderstack.RuntimeContext.Providers
{
    public class ApplicationSettingsProvider : IProvideApplicationSettings
    {
        public ApplicationSettingsProvider(
            IPathManager pathManager
            , IAssemblyManager assemblyManager
            , ITypeManager typeManager
            , IEnvironmentSettings environmentSettings
        )
        {
            _pathManager = pathManager;
            _assemblyManager = assemblyManager;
            _typeManager = typeManager;
            _environmentSettings = environmentSettings;
        }

        /// <summary>
        /// The path manager
        /// </summary>
        private readonly IPathManager _pathManager;

        /// <summary>
        /// The assembly manager
        /// </summary>
        private readonly IAssemblyManager _assemblyManager;

        /// <summary>
        /// The type manager
        /// </summary>
        private readonly ITypeManager _typeManager;

        /// <summary>
        /// The environment settings
        /// </summary>
        private readonly IEnvironmentSettings _environmentSettings;

        /// <summary>
        /// Returns the object model for the root section of an application
        /// settings file.
        /// </summary>
        public TRootSettingSection OfType<TRootSettingSection>() where TRootSettingSection : IApplicationSettingRootSection
        {
            throw new NotImplementedException();
        }
    }
}
