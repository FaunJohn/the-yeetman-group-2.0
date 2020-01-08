using BIF.SWE1.Interfaces;
using System;
using System.Linq;


namespace TemperaturePlugin
{
    public class TemperaturePlugin : IPlugin, IPluginName
    {
        public string Name { get; } = "TemperaturePlugin";

        public float CanHandle(IRequest req)
        {
            if (req == null)
                return 0.0f;

            if (req.IsValid && req.Url.Segments[0].ToLower().Contains("temperature") || (req.IsValid && req.Url.Segments[0].ToLower() == "navi.html"))
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
