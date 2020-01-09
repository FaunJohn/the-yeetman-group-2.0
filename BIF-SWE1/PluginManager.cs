using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection; // for assembly
using System.Text;
using BIF.SWE1.Interfaces;


namespace BIF_SWE1.Uebungen
{
    /// <summary>
    /// Manages all Plugins for the webserver
    /// </summary>
    class PluginManager : IPluginManager
    {
        // ordered plugins list
        private List<IPlugin> _plugins = new List<IPlugin>();

        /// <summary>
        /// Contains all plugins
        /// </summary>
        public IEnumerable<IPlugin> Plugins {
            get { return _plugins; }
            private set
            {
                _plugins = new List<IPlugin>();
                _plugins.AddRange(value); // add to the end if the list
            }

        }

        private string PluginPath { get; set; }

        /// <summary>
        /// Initializes the plugin manager with a path to locate the plugins
        /// </summary>
        /// <param name="pluginPath">Plugin path, default: ./plugins</param>
        public PluginManager(string pluginPath = "./plugins")
        {
            PluginPath = pluginPath;
            LoadPluginsFromPath();
        }

        /// <summary>
        /// Looks for all .ll files in the specified directory (./plugins) that implement IPlugin
        /// and adds them to the Plugin List
        /// </summary>
        public void LoadPluginsFromPath()
        {
            try
            {
                if (Directory.Exists(PluginPath))
                {
                    string[] files = Directory.GetFiles(PluginPath, "*.dll");
                    // selectMany() -> create a single sequence from a sequence in which all of the elements are seperate
                    IEnumerable<IPlugin> allPlugins = files.SelectMany(singlePath =>
                    {
                        // gets all plugin assemblys
                        Assembly assemblyPlugins = LoadPlugin(singlePath);
                        return CreateAllPlugins(assemblyPlugins);
                    }).ToList();

                    Plugins = allPlugins;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Unit Test function
        /// Searches for a plugin with the specified name and loads the types which implement the Plugin (IPlugin) and returns the first one
        /// The assembly should  only contain one specific Plugin
        /// </summary>
        /// <param name="pluginName">Name of the Plugin/Assembly</param>
        /// <returns>First Plugin found in the assembly or null</returns>
        public IPlugin GetPluginFromPath(string pluginName)
        {
            // create plugin path string
            string pluginPath = PluginPath + "/" + pluginName + ".dll";
            // create assembly for plugin
            Assembly pluginAssembly = LoadPlugin(pluginPath);

            if(pluginAssembly != null)
            {
                List<IPlugin> assemblyPlugins = CreateAllPlugins(pluginAssembly).ToList();
                return assemblyPlugins.First(); // OrDefault(); -> exceptions: do not return null!
            } 
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Load existing plugin assembly
        /// </summary>
        /// <param name="pluginPath">Path for loading the plugin assembly</param>
        /// <returns>Assembly or null</returns>
        static Assembly LoadPlugin(string pluginPath)
        {
            // An assembly is the compiled output of your code, typically a DLL. It's the smallest unit of deployment for any .NET project.
            PluginLoadContext loadContext = new PluginLoadContext(pluginPath);
            
            // check if file is available/exists
            if(File.Exists(pluginPath))
            {
                return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginPath)));
            } 
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Creates Instances of all types that implement IPlugin in the assembly
        /// </summary>
        /// <param name="pluginAssembly">Assemly which contains Plugins to implement</param>
        /// <returns>Enumerable of plugins</returns>
        static IEnumerable<IPlugin> CreateAllPlugins(Assembly pluginAssembly)
        {
            // getTypes() returns all types which are defined in this assembly
            foreach (Type type in pluginAssembly.GetTypes())
            {
                // IsAssignableFrom() -> Determines whether an instance of a specified type can be assigned to a variable of the current type.
                if (typeof(IPlugin).IsAssignableFrom(type))
                {
                    // Creates an instance of the specified type using the constructor that best matches the specified parameters.
                    // is tests if the expression can be converted to the specified type and casts it to "result"
                    if (Activator.CreateInstance(type) is IPlugin result)
                    {
                        // You use a yield return statement to return each element one at a time
                        yield return result;
                    }
                }
            }
        }

        /// <summary>
        /// Adds a plugin to the Plugin List
        /// </summary>
        /// <param name="plugin">Plugin that should be added</param>
        public void Add(IPlugin plugin)
        {
            _plugins.Add(plugin);
        }

        /// <summary>
        /// Creates an instance of the specified Plugin and adds it to the Plugin List
        /// </summary>
        /// <param name="plugin">Plugin name (string)</param>
        public void Add(string plugin)
        {
            var type = Type.GetType(plugin);
            var instance = (IPlugin)Activator.CreateInstance(type);
            Add(instance);
        }

        /// <summary>
        /// Clears plugin list
        /// </summary>
        public void Clear()
        {
            Plugins = new List<IPlugin>();
        }

        /// <summary>
        /// HELPER FUNCTION FOR UNIT TESTS
        /// Looks for an assembly with the specified name (within configured plugins directory).
        /// Then returns the first type that implements IPlugin.
        /// (An assembly should only contain one plugin)
        /// </summary>
        /// <param name="pluginName">Plugin name (name of the assembly)</param>
        /// <returns>
        ///     Plugin found: First plugin type that was found in the assembly
        ///     No Plugin found: null
        /// </returns>
        public Type GetPluginTypeFromPath(string pluginName)
        {
            string pluginPath = "plugins/" + pluginName + ".dll";

            // Try to load assembly from specified dll within plugins folder
            Assembly assembly = LoadPlugin(pluginPath);
            if (assembly != null) // A dll could be loaded
            {
                // Get all types that implement IPlugin in the assembly
                // Loop over types in the assembly
                foreach (Type type in assembly.GetTypes())
                {
                    // The plugin implements/is assignable to IPlugin
                    if (typeof(IPlugin).IsAssignableFrom(type))
                    {
                        return type;
                    }
                }
            }

            // No plugin found
            return null;
        }

    }
}
