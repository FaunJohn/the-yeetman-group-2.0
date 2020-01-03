using System.IO;
using System.Linq;
using BIF.SWE1.Interfaces;
using BIF_SWE1;
using BIF_SWE1.Uebungen;

namespace ToLowerPlugin
{
    public class ToLowerPlugin : IPlugin, IPluginName
    {
        public string Name { get; } = "ToLowerPlugin";

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
