using Vanderstack.RuntimeContext.Providers;

namespace Vanderstack.RuntimeContext
{
    public class EnvironmentSettings : IEnvironmentSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentSettings"/> class.
        /// </summary>
        /// <param name="pathManager">The path manager.</param>
        /// <param name="overrideEnvironmentName">Name of the override environment.</param>
        public EnvironmentSettings(IPathManager pathManager, string overrideEnvironmentName)
            : this(new EnvironmentSettingsProvider(pathManager, overrideEnvironmentName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentSettings"/> class.
        /// </summary>
        /// <param name="environmentSettingsProvider">The environment settings provider.</param>
        protected EnvironmentSettings(IProvideEnvironmentSettings environmentSettingsProvider)
        {
            _environmentSettingsProvider = environmentSettingsProvider;
        }

        /// <summary>
        /// Gets the name of the environment.
        /// </summary>
        public string EnvironmentName =>
            _environmentSettingsProvider.EnvironmentName;

        /// <summary>
        /// The environment settings provider
        /// </summary>
        private readonly IProvideEnvironmentSettings _environmentSettingsProvider;
    }
}
