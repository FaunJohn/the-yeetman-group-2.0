using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;
using BIF_SWE1;
using BIF_SWE1.Uebungen;

namespace Uebungen
{
    public class OwnTests
    {
        public void HelloWorld()
        {
            // I'm fine
        }

        public IPlugin GetNavigationPlugin()
        {
            PluginManager pluginManager = new PluginManager();
            return pluginManager.GetPluginFromPath("NaviPlugin");
        }

        public IRequest GetRequest(System.IO.Stream network)
        {
            var req = new Request(network);
            return req;
        }

        public string GetNaviUrl()
        {
            return "/navi";
        }

        public IPlugin GetToLowerPlugin()
        {
            PluginManager pluginManager = new PluginManager();
            return pluginManager.GetPluginFromPath("ToLowerPlugin");
        }

        public string GetToLowerUrl()
        {
            return "lower.html";
        }

        public string GetStaticFileUrl()
        {
            return "static-files";
        }

        public IPlugin GetStaticFilePlugin()
        {
            PluginManager pluginManager = new PluginManager();
            return pluginManager.GetPluginFromPath("StaticFilePlugin");
        }

        public IResponse GetResponse()
        {
            return new Response();
        }
        public string GetStaticFileUrl(string fileName)
        {
            return new Url(fileName).RawUrl;
        }

    }
}
