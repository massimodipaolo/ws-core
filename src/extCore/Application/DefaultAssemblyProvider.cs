// Copyright © 2017 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;

namespace ExtCore.Application
{
    /// <summary>
    /// Implements the <see cref="IAssemblyProvider">IAssemblyProvider</see> interface and represents
    /// default assembly provider that gets assemblies from a specific path and web application dependencies
    /// with the ability to filter the discovered assemblies with the IsCandidateAssembly and
    /// IsCandidateCompilationLibrary predicates.
    /// </summary>
    public class DefaultAssemblyProvider : IAssemblyProvider
    {
        protected ILogger logger;

        /// <summary>
        /// Gets or sets the predicate that is used to filter discovered assemblies from a specific folder
        /// before thay have been added to the resulting assemblies set.
        /// </summary>
        public Func<Assembly, bool> IsCandidateAssembly { get; set; }

        /// <summary>
        /// Gets or sets the predicate that is used to filter discovered libraries from a web application dependencies
        /// before thay have been added to the resulting assemblies set.
        /// </summary>
        public Func<Library, bool> IsCandidateCompilationLibrary { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAssemblyProvider">AssemblyProvider</see> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider that is used to create a logger.</param>
        public DefaultAssemblyProvider(IServiceProvider serviceProvider)
        {
            logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger($"{nameof(ExtCore)}.{nameof(Application)}");
            IsCandidateAssembly = assembly =>
              !assembly.FullName.StartsWith("System", StringComparison.OrdinalIgnoreCase) &&
              !assembly.FullName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase);

            IsCandidateCompilationLibrary = library =>
              !library.Name.StartsWith("mscorlib", StringComparison.OrdinalIgnoreCase) &&
              !library.Name.StartsWith("netstandard", StringComparison.OrdinalIgnoreCase) &&
              !library.Name.StartsWith("System", StringComparison.OrdinalIgnoreCase) &&
              !library.Name.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase) &&
              !library.Name.StartsWith("WindowsBase", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Discovers and then gets the discovered assemblies from a specific folder and web application dependencies.
        /// </summary>
        /// <param name="path">The extensions path of a web application.</param>
        /// <param name="includingSubpaths">
        /// Determines whether a web application will discover and then get the discovered assemblies from the subfolders
        /// of a specific folder recursively.
        /// </param>
        /// <returns>The discovered and loaded assemblies.</returns>
        public IEnumerable<Assembly> GetAssemblies(string path, bool includingSubpaths)
        {
            List<Assembly> assemblies = new();
            GetAssembliesFromDependencyContext(assemblies);
            GetAssembliesFromPath(assemblies, path, includingSubpaths);
            return assemblies.GroupBy(_ => _.FullName?.ToLowerInvariant()).Select(_ => _.First());
        }

        private void GetAssembliesFromDependencyContext(List<Assembly> assemblies)
        {
            foreach (string assemblyName in DependencyContext.Default.CompileLibraries?.Where(IsCandidateCompilationLibrary)?.Select(_ => _.Name))
            {
                Assembly assembly = null;
                try
                {
                    assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName));
                    assemblies.Add(assembly);
                    logger.LogInformation("Assembly '{assembly}' is discovered and loaded from '{context}'", assembly.FullName, nameof(DependencyContext));
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Error loading assembly '{assembly}' from '{context}'", assemblyName, nameof(DependencyContext));
                }
            }
        }

        private void GetAssembliesFromPath(List<Assembly> assemblies, string path, bool includingSubpaths)
        {
            if (Directory.Exists(path ?? ""))
            {
                foreach (string extensionPath in Directory.EnumerateFiles(path, "*.dll"))
                {
                    try
                    {
                        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(extensionPath);

                        if (IsCandidateAssembly(assembly))
                        {
                            assemblies.Add(assembly);
                            logger.LogInformation("Assembly '{assembly}' is discovered and loaded from '{context}'", assembly.FullName, path);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Error loading assembly '{assembly}' from '{context}'", extensionPath, path);
                    }
                }

                GetAssemblyFromSubPath(assemblies, path, includingSubpaths);
            }
            else
                logger.LogWarning("Discovering and loading assemblies from path '{path}' skipped: path not found", path);
        }

        private void GetAssemblyFromSubPath(List<Assembly> assemblies, string path, bool includingSubpaths)
        {
            if (includingSubpaths)
                foreach (string subpath in Directory.GetDirectories(path))
                    GetAssembliesFromPath(assemblies, subpath, includingSubpaths);
        }
    }
}