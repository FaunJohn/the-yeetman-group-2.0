﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;
using BIF_SWE1.Uebungen;

namespace Uebungen
{
    public class UEB4
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

        public IResponse GetResponse()
        {
            return new Response();
        }
    }
}
