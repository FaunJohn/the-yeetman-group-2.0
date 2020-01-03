using BIF.SWE1.Interfaces;
using BIF_SWE1.Uebungen;
using System.Linq;
using System;
using System.IO;

namespace SFP
{
    public class StaticFilePlugin : IPlugin, IPluginName
    {
        public string Name { get; } = "StaticFilePlugin";

        public float CanHandle(IRequest req)
        {
            if (req == null)
                return 0.0f;

            if (req.IsValid)
            {
                return 0.2f;
            }

            return 0.0f;
        }

        public IResponse Handle(IRequest request)
        {
            string filePath = request.Url.Path;
            Response response = new Response();

            if (File.Exists("./static-files/" + filePath))
            {
                response.StatusCode = 200;
                response.SetContent(File.OpenRead("./static-files/" + filePath));
                string fileExtension = Path.GetExtension("./static-files/" + filePath).Trim('.');
                Console.WriteLine("File Ext: " + fileExtension);
                Console.WriteLine("File Path: " + "./static-files/" + filePath);
                response.ContentType = response.KnownFileExtensions[fileExtension] ?? "text/plain";
            }
            else
            {
                response.StatusCode = 404;
                response.SetContent(response.Status);
            }
            return response;
        }
    }
}
