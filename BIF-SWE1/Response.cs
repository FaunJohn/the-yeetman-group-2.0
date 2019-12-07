using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;

namespace BIF_SWE1.Uebungen
{
    class Response : IResponse
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

        public string ServerHeader 
        { 
            // to implement
            get 
            {
                return "x";
            }
            set
            { 

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
            throw new NotImplementedException();
        }

        public void SetContent(string content)
        {
            throw new NotImplementedException();
        }

        public void SetContent(byte[] content)
        {
            throw new NotImplementedException();
        }

        public void SetContent(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
