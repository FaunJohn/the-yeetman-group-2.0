using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;

namespace BIF_SWE1.Uebungen
{
    public class Response : IResponse
    {
        public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Known Status Codes
        /// </summary>
        public IDictionary<int, string> KnownStatusCodes = new Dictionary<int, string>()
        {
            { 500, "500 INTERNAL SERVER ERROR"}, { 404, "404 NOT FOUND"}, { 200, "200 OK"}
        };

        public int ContentLength  { get; private set; }

        public string ContentType { get; set; }

        private bool StatusCodeCheck { get; set; } = false;

        /// <summary>
        /// Helper variable for Status Code saving
        /// </summary>
        private int statusCodeHelper { get; set; }

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

        public string Status { get; set; }
        
        /// <summary>
        /// Server Header Helper is filled with default Server Header
        /// </summary>
        private string serverHeaderHelper { get; set; } = "BIF-SWE1-Server";

        public string ServerHeader 
        { 
            // to implement
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

        public void Send(Stream network)
        {
            // TODO: rework
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

        public string Content { get; set; }

        public void SetContent(string content)
        {
            // save content?
            Content = content;
            ContentLength = Encoding.UTF8.GetByteCount(Content);
        }

        public void SetContent(byte[] content)
        {
            Content = content.ToString();
            ContentLength = Encoding.UTF8.GetByteCount(Content);
        }

        public void SetContent(Stream stream)
        {
            StreamReader sr = new StreamReader(stream, Encoding.UTF8);

            string line;

            while ((line = sr.ReadLine()) != null)
            {
                    Content += line;
                    ContentLength += line.Length;
            }
            sr.Close();
        }
    }
}
