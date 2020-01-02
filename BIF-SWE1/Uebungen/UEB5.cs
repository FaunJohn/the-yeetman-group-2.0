using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;
using BIF_SWE1;
using BIF_SWE1.Uebungen;

namespace Uebungen
{
    public class UEB5
    {
        private string StaticFileFolder;


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

        public IPlugin GetStaticFilePlugin()
        {
            PluginManager pm = new PluginManager();
            return pm.GetPluginFromPath("StaticFilePlugin");
        }

        public string GetStaticFileUrl(string fileName)
        {
            return new Url(fileName).RawUrl;
        }

        public void SetStatiFileFolder(string folder)
        {
            // do something?
            StaticFileFolder = folder;
        }
    }
}
