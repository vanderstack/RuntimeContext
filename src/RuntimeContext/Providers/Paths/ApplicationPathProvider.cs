using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Vanderstack.RuntimeContext.Providers
{
    public class ApplicationPathProvider : IProvidePaths
    {
        /*
            Based on the research below, we should base our dll path on the
            result of Assembly.GetEntryAssembly().CodeBase. This will ensure
            we *always* get the path where the dll files are located
            https://stackoverflow.com/questions/52797/how-do-i-get-the-path-of-the-assembly-the-code-is-in

            Having determined the Entry Assembly Path we can search for the
            substring "\bin\debug\" which will be present when running in a
            non deployment configuration such as VS Debugger, VS Test Runner,
            and cmd dotnet test.

            If the path does contain the token we are in a debug configuration
            and we need to walk up the path until we find the *.sln file

            If the path does not contain the token we are in a deployment
            configuration and the current path is the root.
              When missing files are discovered while using a deployment
              configuration we must change the deployment process to ensure
              they are copied during the deployment.
              This is not a bug of the Application Path Provider, but rather
              a bug of the deployment process which was used.

            What paths do we care about?
                Application Root (all resources exist inside)
                Web Content (wwwroot)
                Settings Files (*.json)
                Binary Files (*.dll)

            How do we handle adding new paths?
                We dont. Instead we extend the types of paths we care about to
                include new filter criteria. We detect the Application Root, and
                from there we identify all file types of interest such as settings,
                binaries, or the newly defined criteria.

            But don't we want to allow a developer to extend the provider by
            defining a class which implements some path provider interface?
                No. We cannot be open to extension from external
                assemblies when we cannot import them and we import external
                assemblies using the paths determined by the path provider.
                It is standard practice to require external assemblies be installed
                by copying them to a location inside the Application Root.

            Candidate Implementation Options for finding Application Root:
                Directory.GetCurrentDirectory()
                Assembly.GetEntryAssembly().Location

            What does Directory.GetCurrentDirectory() do?
                Run Web Project from VS:
                C:\Solution\WebProject\Solution.WebProject.Web

                Run Web Project from cmd:
                C:\Solution\WebProject\Solution.WebProject.Web

                Run dotnet test from VS / cmd path C:\Solution\WebProject\Solution.WebProject.Tests
                C:\Solution\WebProject\Solution.WebProject.Tests\bin\Debug\netcoreapp1.1.

                Run dotnet test from VS / cmd path C:\Solution\ServiceLibrary\Solution.ServiceLibrary.Tests
                C:\Solution\ServiceLibrary\Solution.ServiceLibrary.Tests\bin\Debug\netcoreapp1.1.

                Run dotnet run from cmd path C:\Publish:
                C:\Publish

            What does Assembly.GetEntryAssembly().Location do?
                Run Web Project from VS:
                C:\Solution\WebProject\Solution.WebProject.Web\bin\Debug\netcoreapp1.1\Solution.WebProject.Web.dll

                Run dotnet test from cmd path C:\Solution\ServiceLibrary\Solution.ServiceLibrary.Tests
                C:\Solution\ServiceLibrary\Solution.ServiceLibrary.Tests\bin\Debug\netcoreapp1.1.

                Run dotnet run from cmd path C:\Publish:
                C:\Publish\Solution.WebProject.Web.dll
        */

        /// <summary>
        /// Constructs a new instance of Application Path Provider
        /// </summary>
        public ApplicationPathProvider()
        {
            // Here we need to initialize each of our private readonly
            // lazilly initialized paths with resolution strategies.

            _entryAssembly = InitializeEntryAssembly();
            _applicationRoot = InitializeApplicationRoot();
            _webContentRoot = InitializeWebContentRoot();
            _settings = InitializeSettings();
            _assemblies = InitializeAssemblies();
        }

        /// <summary>
        /// Gets the path of the working directory.
        /// </summary>
        ///
        /// <remarks>
        /// This value will change depending on how the application is launched.
        /// Please favor ApplicationRoot.
        /// </remarks>
        public string WorkingDirectory =>
            Directory.GetCurrentDirectory();

        /// <summary>
        /// Gets the path of the entry assembly.
        /// </summary>
        ///
        /// <remarks>
        /// This value will change depending on how the application is launched.
        /// Please favor ApplicationRoot.
        /// </remarks>
        public string EntryAssembly =>
            _entryAssembly.Value;
        private readonly Lazy<string> _entryAssembly;

        /// <summary>
        /// Gets the path of the application root.
        /// </summary>
        ///
        /// <remarks>
        /// The Application Root contains all application resources.
        /// </remarks>
        public string ApplicationRoot =>
            _applicationRoot.Value;
        private readonly Lazy<string> _applicationRoot;

        /// <summary>
        /// Gets the path of the web content.
        /// </summary>
        ///
        /// <remarks>
        /// The Web Content Root contains all files served by the Api.
        /// </remarks>
        public string WebContentRoot =>
            _webContentRoot.Value;
        private readonly Lazy<string> _webContentRoot;

        // todo: update comments about settins and assembly filenames

        /// <summary>
        /// Gets the path for each settings file within the Application Root.
        /// </summary>
        ///
        /// <remarks>
        /// Settings files are configured to end in .json
        /// </remarks>
        public IEnumerable<string> Settings =>
            _settings.Value;
        private readonly Lazy<IEnumerable<string>> _settings;

        /// <summary>
        /// Gets the path for each assembly file within the Application Root.
        /// </summary>
        ///
        /// <remarks>
        /// Assembly files are configured to end in .dll
        /// </remarks>
        public IEnumerable<string> Assemblies =>
            _assemblies.Value;
        private readonly Lazy<IEnumerable<string>> _assemblies;

        private Lazy<IEnumerable<string>> InitializeAssemblies()
        {
            // The assembly files will always be inside of the Application Root
            // and have been configured to use the ".dll" extension
            return new Lazy<IEnumerable<string>>(
                valueFactory: () =>
                {
                    return
                        Directory
                        .GetFiles(
                            ApplicationRoot
                            , "*.dll"
                            , SearchOption.AllDirectories
                        );
                }
                , mode: LazyThreadSafetyMode.ExecutionAndPublication
            );
        }

        private Lazy<IEnumerable<string>> InitializeSettings()
        {
            // The settings files will always be inside of the Application Root
            // and have been configured to use the ".json" extension
            return new Lazy<IEnumerable<string>>(
                valueFactory: () =>
                {
                    return
                        Directory
                        .GetFiles(
                            ApplicationRoot
                            , "*.json"
                            , SearchOption.AllDirectories
                        );
                }
                , mode: LazyThreadSafetyMode.ExecutionAndPublication
            );
        }

        private Lazy<string> InitializeWebContentRoot()
        {
            // todo: close US-1801
            // The web content root is always inside of the Application Root
            // and has been configured with the value "wwwroot"
            return new Lazy<string>(
                valueFactory: () =>
                {
                    return
                        Directory
                        .GetDirectories(
                            ApplicationRoot
                            , "wwwroot"
                            , SearchOption.AllDirectories
                        )
                        // There should only be a single Web Content root
                        // within our application
                        .SingleOrDefault();
                }
                , mode: LazyThreadSafetyMode.ExecutionAndPublication
            );
        }

        private Lazy<string> InitializeApplicationRoot()
        {
            // Using our entry assembly path as a starting point we know that
            // we are either in a debug or a deployment configuration.
            // When in a debug configuration our path will contain the token
            // "\debug\bin\". We can search for this token to determine the
            // configuration. In a deployment configuration all of our resources
            // will be in the same path so we can use the Entry Assembly path.
            // In a debug config we can walk up the path until we reach the
            // directory which contains the solution file, which will be the
            // root directory.

            return new Lazy<string>(
                valueFactory: () =>
                {
                    var candidateDirectory = EntryAssembly;

                    var isReleaseConfiguration =
                        !candidateDirectory
                        .Contains(@"\bin\Debug\");

                    if (isReleaseConfiguration)
                    {
                        return candidateDirectory;
                    }
                    else
                    {
                        while (!string.IsNullOrEmpty(candidateDirectory))
                        {
                            if (
                                Directory.GetFiles(
                                    candidateDirectory
                                    , "*.sln"
                                    , SearchOption.TopDirectoryOnly
                                )
                                .Length != 0
                            )
                            {
                                return candidateDirectory;
                            }

                            candidateDirectory = Directory.GetParent(candidateDirectory)?.FullName;
                        }

                        throw new Exception("The Path for the Application Root could not be determined.");
                    }
                }
                , mode: LazyThreadSafetyMode.ExecutionAndPublication
            );
        }

        private Lazy<string> InitializeEntryAssembly()
        {
            // By using Assembly.GetEntryAssembly().CodeBase to determine the
            // assembly path and using that to compute the root path, we are
            // able to eliminate the path inconsistencies that would arise when
            // running test suites as opposed to launching from VS or running
            // in a deployment configuration where all resources are flattened.
            return new Lazy<string>(
                valueFactory: () =>
                {
                    //https://stackoverflow.com/questions/52797/how-do-i-get-the-path-of-the-assembly-the-code-is-in
                    var codeBase = Assembly.GetEntryAssembly().CodeBase;
                    var uri = new UriBuilder(codeBase);
                    var path = Uri.UnescapeDataString(uri.Path);

                    return Path.GetDirectoryName(path);
                }
                , mode: LazyThreadSafetyMode.ExecutionAndPublication
            );
        }
    }
}
