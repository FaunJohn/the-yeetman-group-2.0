using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.Loader;

namespace BIF_SWE1
{
    /// <summary>
    /// Extends The Assembly Load Context with plugin specific Code
    /// </summary>
    class PluginLoadContext : AssemblyLoadContext
    {
        /// <summary>
        /// The AssemblyLoadContext exists primarily to provide assembly loading isolation. It allows multiple versions of the same assembly to be loaded within a single process. 
        /// Represents the runtime's concept of a scope for assembly loading.
        /// Every .NET Core application implicitly uses the AssemblyLoadContext. It's the runtime's provider for locating and loading dependencies. Whenever a dependency is loaded, an AssemblyLoadContext instance is invoked to locate it.
        /// It provides a service of locating, loading, and caching managed assemblies and other dependencies.
        /// To support dynamic code loading and unloading, it creates an isolated context for loading code and its dependencies in their own AssemblyLoadContext instance.
        /// </summary>
        private AssemblyDependencyResolver _resolver;

        /// <summary>
        /// Setup the AssemblyDependencyResolver
        /// </summary>
        /// <param name="pluginPath">Given Plugin Path</param>
        public PluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        /// <summary>
        /// Load the assembly contents
        /// </summary>
        /// <param name="assemblyName">Unique identifier of an assembly</param>
        /// <returns>Assembly of type Assembly</returns>
        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }
    }
}