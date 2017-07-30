using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Vanderstack.RuntimeContext.Extensions;

namespace Vanderstack.RuntimeContext.Providers
{
    public class EnvironmentSettingsProvider : IProvideEnvironmentSettings
    {
        public EnvironmentSettingsProvider(
            IPathManager pathManager
            , string overrideEnvironmentName
        )
        {
            _pathManager = pathManager;
            _overrideEnvironmentName = overrideEnvironmentName;

            _environmentNameResolver = InitializeEnvironmentNameResolver();
        }

        /// <summary>
        /// The path manager
        /// </summary>
        private readonly IPathManager _pathManager;

        /// <summary>
        /// The override environment name
        /// </summary>
        private readonly string _overrideEnvironmentName;

        /// <summary>
        /// Gets the environment name.
        /// </summary>
        public string EnvironmentName =>
            _environmentNameResolver.Value();

        /// <summary>
        /// The environment name.
        /// </summary>
        private readonly Lazy<Func<string>> _environmentNameResolver;

        /// <summary>
        /// Initializes the name of the environment.
        /// </summary>
        private Lazy<Func<string>> InitializeEnvironmentNameResolver()
        {
            if (!string.IsNullOrEmpty(_overrideEnvironmentName))
            {
                return new Lazy<Func<String>>(
                    valueFactory: () =>
                    {
                        return () => _overrideEnvironmentName;
                    }
                    , mode: LazyThreadSafetyMode.ExecutionAndPublication
                );
            }

            return new Lazy<Func<string>>(
                valueFactory: () =>
                {
                    var environmentSettingsPath =
                        _pathManager
                        .Settings
                        .Single(settingsPath =>
                            settingsPath.Contains(nameof(EnvironmentSettings))
                        );

                    var basePath = environmentSettingsPath.BeforeLast(Path.DirectorySeparatorChar.ToString());
                    var filename = environmentSettingsPath.AfterLast(Path.DirectorySeparatorChar.ToString());

                    var configuration =
                        new ConfigurationBuilder()
                        .SetBasePath(basePath)
                        .AddJsonFile(
                            path: filename
                            , optional: false
                            , reloadOnChange: true)
                        .Build();

                    var serviceCollection = new ServiceCollection();
                    serviceCollection.AddOptions();
                    serviceCollection.Configure<EnvironmentSettingsSection>(
                        configuration
                    );

                    var serviceProvider = serviceCollection.BuildServiceProvider();

                    Func<EnvironmentSettingsSection> settingsSectionResolver = () =>
                    {
                        using (var scope = serviceProvider.CreateScope())
                        {
                            return
                                scope
                                .ServiceProvider
                                .GetService<IOptionsSnapshot<EnvironmentSettingsSection>>()
                                .Value;
                        }
                    };

                    return () => settingsSectionResolver().EnvironmentName;
                }
                , mode: LazyThreadSafetyMode.ExecutionAndPublication
            );
        }
    }

    public class EnvironmentSettingsSection
    {
        public string EnvironmentName { get; set; }
    }
}
