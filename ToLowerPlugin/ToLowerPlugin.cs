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
            if (req == null)
            {
                return new Response { StatusCode = 404 };
            }

            if(req.IsValid && req.ContentString == "text=")
            {
                Response response = new Response { StatusCode = 200 };
                response.SetContent("Bitte geben Sie einen Text ein");
                return response;
            }

            // returns lower case request content string
            else if (req.IsValid && req.Url.Segments.Length == 1 && req.Url.Segments[0].ToLower() == "tolower.html" && req.Method == "POST")
            {
                Response response = new Response { StatusCode = 200 };
                response.SetContent(req.ContentString.ToLower());
                return response;
            }
            else
            {
                return new Response { StatusCode = 404 };
            }
        }


    }
}
