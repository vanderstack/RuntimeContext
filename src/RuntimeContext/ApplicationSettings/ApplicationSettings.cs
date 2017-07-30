using System;
using Vanderstack.RuntimeContext.Providers;

namespace Vanderstack.RuntimeContext
{
    public class ApplicationSettings : IApplicationSettings
    {
        /// <summary>
        /// Creates a new instance of the Application Settings.
        /// </summary>
        /// <param name="pathManager">The Path Manager.</param>
        /// <param name="assemblyManager">The Assembly Manager.</param>
        /// <param name="typeManager">The Type Manager.</param>
        /// <param name="environmentSettings">The Environment Settings.</param>
        public ApplicationSettings(
            IPathManager pathManager
            , IAssemblyManager assemblyManager
            , ITypeManager typeManager
            , IEnvironmentSettings environmentSettings
        ) : this(
            new ApplicationSettingsProvider(
                pathManager
                , assemblyManager
                , typeManager
                , environmentSettings
            )
        )
        {
        }

        /// <summary>
        /// Creates a new instance of the ApplicationSettings.
        /// </summary>
        /// <param name="applicationSettingsProvider">The application settings provider.</param>
        protected ApplicationSettings(ApplicationSettingsProvider applicationSettingsProvider)
        {
            _applicationSettingsProvider = applicationSettingsProvider;
        }
        
        /// <summary>
        /// Provides the Application Settings.
        /// </summary>
        private readonly ApplicationSettingsProvider _applicationSettingsProvider;

        /// <summary>
        /// Returns the object model for the root section of an application
        /// settings file.
        /// </summary>
        public TRootSettingSection OfType<TRootSettingSection>()
            where TRootSettingSection : IApplicationSettingRootSection =>
            _applicationSettingsProvider.OfType<TRootSettingSection>();
    }
}
