using System;
using System.Collections.Generic;
using System.Text;
using BIF.SWE1.Interfaces;


namespace BIF_SWE1.Uebungen
{
    class PluginManager : IPluginManager
    {
        public IEnumerable<IPlugin> Plugins => throw new NotImplementedException();

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
