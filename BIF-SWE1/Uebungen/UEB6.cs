using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;
using BIF_SWE1.Uebungen;

namespace Uebungen
{
    public class UEB6
    {
        public void HelloWorld()
        {
        }

        public IPluginManager GetPluginManager()
        {
            return new PluginManager();
        }

        public IRequest GetRequest(System.IO.Stream network)
        {
            return new Request(network);
        }

        public string GetNaviUrl()
        {
            return "/navi.html";
        }

        public IPlugin GetNavigationPlugin()
        {
            PluginManager pluginManager = new PluginManager();
            return pluginManager.GetPluginFromPath("NaviPlugin");
        }

        public IPlugin GetTemperaturePlugin()
        {
            throw new NotImplementedException();
        }

        public string GetTemperatureRestUrl(DateTime from, DateTime until)
        {
            throw new NotImplementedException();
        }

        public string GetTemperatureUrl(DateTime from, DateTime until)
        {
            throw new NotImplementedException();
        }

        public IPlugin GetToLowerPlugin()
        {
            PluginManager pluginManager = new PluginManager();
            return pluginManager.GetPluginFromPath("ToLowerPlugin");
        }

        public string GetToLowerUrl()
        {
            return "/lower.html";
        }

        public IPlugin GetNavigationPluginWithoutLoadingMap()
        {
            PluginManager pluginManager = new PluginManager();
            Type type = pluginManager.GetPluginTypeFromPath("NaviPlugin");
            IPlugin plugin = Activator.CreateInstance(type, new object[] { false }) as IPlugin;
            return plugin;
        }
    }
}
