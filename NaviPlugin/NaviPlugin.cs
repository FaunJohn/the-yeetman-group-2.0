using BIF.SWE1.Interfaces;
using System;
using System.Linq;


namespace NaviPlugin
{
    public class NaviPlugin : IPlugin, IPluginName
    {
        public string Name { get; } = "NaviPlugin";

        public float CanHandle(IRequest req)
        {
            if (req == null)
                return 0.0f;

            if (req.IsValid && req.Url.Segments[0].ToLower().Contains("tolower"))
            {
                return 1.0f;
            }

            return 0.0f;
        }
        public IResponse Handle(IRequest req)
        {
            return null;
        }
    }
}
