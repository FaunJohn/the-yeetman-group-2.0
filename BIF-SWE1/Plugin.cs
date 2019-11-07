using System;
using System.Collections.Generic;
using System.Text;
using BIF.SWE1.Interfaces;


namespace BIF_SWE1.Uebungen
{
    class Plugin : IPlugin
    {
        public float CanHandle(IRequest req)
        {
            throw new NotImplementedException();
        }

        public IResponse Handle(IRequest req)
        {
            throw new NotImplementedException();
        }
    }
}
