using BIF.SWE1.Interfaces;
using BIF_SWE1.Uebungen;
using System.Linq;
using System;
using System.IO;

namespace SFP
{
    public class StaticFilePlugin : IPlugin, IPluginName
    {
        /// <summary>
        /// A plugin for loading static files from the webserver
        /// </summary>
        public string Name { get; } = "StaticFilePlugin";

        /// <summary>
        /// Checks if the given Plugin can handle a request
        /// </summary>
        /// <param name="req">Given request</param>
        /// <returns>Float</returns>
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

        /// <summary>
        /// Handles the request, returns the static file
        /// </summary>
        /// <param name="req"></param>
        /// <returns>Valid response with response code...</returns>
        public IResponse Handle(IRequest request)
        {
            string filePath = request.Url.Path;
            Response response = new Response();
            
            if(request.Url.RawUrl == "/")
            {
                filePath = "index.html";
            }
            if (File.Exists("./static-files/" + filePath))
            {
                Console.WriteLine("./static-files/" + filePath);
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
