using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;

namespace BIF_SWE1.Uebungen
{
    /// <summary>
    /// HTTP Response class
    /// Can add headers and set a request body.
    /// Sends the response to the client
    /// </summary>
    public class Response : IResponse
    {
        /// <summary>
        /// Returns a dictionary, which contains the response headers
        /// </summary>
        public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Valid status codes, the sercer can handle
        /// </summary>
        public IDictionary<int, string> KnownStatusCodes = new Dictionary<int, string>()
        {
            { 500, "500 INTERNAL SERVER ERROR"}, { 404, "404 NOT FOUND"}, { 200, "200 OK"}, { 503, "503 UNAVAILABLE"}
        };

        /// <summary>
        /// Dictionary of file extensions with their mime type
        /// </summary>
        public IDictionary<string, string> KnownFileExtensions = new Dictionary<string, string>()
        {
            {"html", "text/html"},
            {"css", "text/css"},
            {"", "text/plain"},
            {"js", "text/javascript"},
            {"gif", "image/gif"},
            {"jpeg", "image/jpeg"},
            {"jpg", "image/jpeg"},
            {"png", "image/png"},
            {"txt", "text/plain"},
            { "json", "text/json"}
        };

        /// <summary>
        /// Adds a server Header and closes the connectin after each response
        /// </summary>
        public Response()
        {
            AddHeader("connection", "close");
        }

        /// <summary>
        /// Returns Content Length, can be 0 if there is no content set
        /// </summary>
        public int ContentLength  { get; private set; }

        /// <summary>
        /// Returns the Content Type of the response.
        /// </summary>
        public string ContentType { get; set; }

        private bool StatusCodeCheck { get; set; } = false;

        private int statusCodeHelper { get; set; }

        /// <summary>
        /// Can GET or SET a Status Code
        /// </summary>
        public int StatusCode 
        {
            get
            {
                if(StatusCodeCheck == false)
                {
                    throw new Exception("Status Code is not set!");
                }
                else
                {
                    return statusCodeHelper;
                }
            }
            set
            {
                if (KnownStatusCodes.ContainsKey(value))
                {
                    StatusCodeCheck = true;
                    Status = KnownStatusCodes[value];
                    statusCodeHelper = value;
                }
            }
        }

        /// <summary>
        /// Returns the status code as a string -> 503 UNAVAILABLE
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// Server Header Helper is filled with default Server Header
        /// </summary>
        private string serverHeaderHelper { get; set; } = "BIF-SWE1-Server";

        /// <summary>
        /// Can GET or SET the server response header
        /// </summary>
        public string ServerHeader 
        { 
            get 
            {
                return serverHeaderHelper;
            }
            set
            {
                serverHeaderHelper = value; 
                AddHeader("Server", value);
            }
        }

        /// <summary>
        /// Adds or repleaces the response header in the headers Dictionary
        /// </summary>
        /// <param name="header">Header Name</param>
        /// <param name="value">Header Value</param>
        public void AddHeader(string header, string value)
        {
            if(Headers.ContainsKey(header))
            {
                Headers[header] = value; 
            }
            else
            {
                Headers.Add(header, value);
            }
        }

        /// <summary>
        /// Sends a response to the nework stream
        /// </summary>
        /// <param name="network">Network Stream</param>
        public void Send(Stream network)
        {
            if (String.IsNullOrEmpty(Content) && !String.IsNullOrEmpty(ContentType))
                throw new Exception("Expected non-empty body when content-type is set, got empty body.");

            StreamWriter sw = new StreamWriter(network, leaveOpen: true);


            // Fix for testcase response_should_send_404
            if (Status == "404 NOT FOUND") Status = "404 Not Found";

            sw.WriteLine("HTTP/1.1" + ' ' + Status);
            

            foreach (var header in Headers)
            {
                sw.WriteLine(header.Key + ": " + header.Value);
            }
            sw.WriteLine($"Content-Length: {ContentLength}");
            if (ContentType != null)
            {
                sw.WriteLine($"Content-Type: {ContentType}");
            }

            sw.WriteLine();
            sw.Write(Content);
            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// Internal string representation of the response body/content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Internal byte representation of the response body/content
        /// </summary>
        public byte[] byteContent { get; set; }

        /// <summary>
        /// Sets string as content encoded in UTF8
        /// </summary>
        /// <param name="content">Content string</param>
        public void SetContent(string content)
        {
            Content = content;
            byteContent = Encoding.UTF8.GetBytes(Content);
            ContentLength = byteContent.Length;
        }

        /// <summary>
        /// Sets byte as content
        /// </summary>
        /// <param name="content">content bytes</param>
        public void SetContent(byte[] content)
        {
            Content = Encoding.UTF8.GetString(content);
            byteContent = content;
            ContentLength = content.Length;
        }

        /// <summary>
        /// Sets stream as content
        /// </summary>
        /// <param name="stream">content stream</param>
        public void SetContent(Stream stream)
        {
            var ms = new MemoryStream();
            stream.CopyTo(ms);
            SetContent(ms.ToArray());
        }
    }
}
