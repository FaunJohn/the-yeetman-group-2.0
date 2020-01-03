using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection; // for assembly
using System.Text;
using BIF.SWE1.Interfaces;


namespace BIF_SWE1.Uebungen
{
    class PluginManager : IPluginManager
    {
        // ordered plugins list
        private List<IPlugin> _plugins = new List<IPlugin>();

        public IEnumerable<IPlugin> Plugins {
            get { return _plugins; }
            private set
            {
                _plugins = new List<IPlugin>();
                _plugins.AddRange(value); // add to the end if the list
            }

        }

        private string PluginPath { get; set; }

        // TODO:
        public PluginManager(string pluginPath = "./plugins")
        {
            PluginPath = pluginPath;
            LoadPluginsFromPath();
        }

        public void LoadPluginsFromPath()
        {
            try
            {
                if (Directory.Exists(PluginPath))
                {
                    string[] files = Directory.GetFiles(PluginPath, "*.dll");
                    // selectMany() -> create a single sewuence from a sequence in which all of the elements are seperate
                    IEnumerable<IPlugin> allPlugins = files.SelectMany(singlePath =>
                    {
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

        // load existing plugin assembly
        static Assembly LoadPlugin(string pluginPath)
        {
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

        static IEnumerable<IPlugin> CreateAllPlugins(Assembly pluginAssembly)
        {
            // getTypes() returns all types which are defines in this assembly
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

        public void Add(IPlugin plugin)
        {
            _plugins.Add(plugin);
        }

        public void Add(string plugin)
        {
            var type = Type.GetType(plugin);
            var instance = (IPlugin)Activator.CreateInstance(type);
            Add(instance);
        }

        public void Clear()
        {
            Plugins = new List<IPlugin>();
        }
    }
}
