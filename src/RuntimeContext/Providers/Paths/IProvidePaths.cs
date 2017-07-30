using System;
using System.Collections.Generic;

namespace Vanderstack.RuntimeContext.Providers
{
    public interface IProvidePaths
    {
        /// <summary>
        /// Gets the path of the working directory.
        /// </summary>
        ///
        /// <remarks>
        /// This value will change depending on how the application is launched.
        /// Please favor ApplicationRoot.
        /// </remarks>
        string WorkingDirectory { get; }

        /// <summary>
        /// Gets the path of the entry assembly.
        /// </summary>
        ///
        /// <remarks>
        /// This value will change depending on how the application is launched.
        /// Please favor ApplicationRoot.
        /// </remarks>
        string EntryAssembly { get; }

        /// <summary>
        /// Gets the path of the application root.
        /// </summary>
        ///
        /// <remarks>
        /// The Application Root contains all application resources.
        /// </remarks>
        string ApplicationRoot { get; }

        /// <summary>
        /// Gets the path of the web content.
        /// </summary>
        ///
        /// <remarks>
        /// The Web Content Root contains all files served by the Api.
        /// </remarks>
        string WebContentRoot { get; }

        /// <summary>
        /// Gets the path for each setting files within the Application Root.
        /// </summary>
        ///
        /// <remarks>
        /// Settings files are configured to end in .json
        /// </remarks>
        IEnumerable<String> Settings { get; }

        /// <summary>
        /// Gets the path for each assembly file within the Application Root.
        /// </summary>
        ///
        /// <remarks>
        /// Assembly files are configured to end in .dll
        /// </remarks>
        IEnumerable<String> Assemblies { get; }
    }
}
