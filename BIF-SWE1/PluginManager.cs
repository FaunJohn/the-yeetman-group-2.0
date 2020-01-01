using System;
using System.Collections.Generic;
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

        public void Add(IPlugin plugin)
        {
            throw new NotImplementedException();
        }

        public void Add(string plugin)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
