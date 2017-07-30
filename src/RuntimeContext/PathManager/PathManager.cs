using System.Collections.Generic;
using Vanderstack.RuntimeContext.Providers;

namespace Vanderstack.RuntimeContext
{
    public class PathManager : IPathManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathManager"/> class.
        /// </summary>
        public PathManager()
            : this(new ApplicationPathProvider())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathManager"/> class.
        /// </summary>
        /// <param name="pathProvider">The path provider.</param>
        protected PathManager(IProvidePaths pathProvider)
        {
            _pathProvider = pathProvider;
        }

        /// <summary>
        /// The path provider
        /// </summary>
        private readonly IProvidePaths _pathProvider;

        /// <summary>
        /// Gets the working directory.
        /// </summary>
        public string WorkingDirectory =>
            _pathProvider.WorkingDirectory;

        /// <summary>
        /// Gets the entry assembly.
        /// </summary>
        public string EntryAssembly =>
            _pathProvider.EntryAssembly;


        /// <summary>
        /// Gets the application root.
        /// </summary>
        public string ApplicationRoot =>
            _pathProvider.ApplicationRoot;

        /// <summary>
        /// Gets the web content root.
        /// </summary>
        public string WebContentRoot =>
            _pathProvider.WebContentRoot;

        /// <summary>
        /// Gets the settings.
        /// </summary>
        public IEnumerable<string> Settings =>
            _pathProvider.Settings;

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        public IEnumerable<string> Assemblies =>
            _pathProvider.Assemblies;
    }
}
