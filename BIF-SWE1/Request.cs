using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;


namespace BIF_SWE1.Uebungen
{
    class Request : IRequest
    {
        public Request(Stream requestStream)
        {
            ReqStreamReader = new StreamReader(requestStream);
            ProcessRequest();
        }

        private StreamReader ReqStreamReader { get; }

        private string[] AllowedMethods { get; } = { "GET", "POST" };

        private void ProcessRequest()
        {
            string requestLine;
            // set isValid to false if the request is null
            if((requestLine = ReqStreamReader.ReadLine()) == null)
            {
                IsValid = false;
            } 
            else
            {
                Console.WriteLine("Begin");
                Console.WriteLine(requestLine);
                Console.WriteLine("END");

                // should look like this: GET /favicon.ico HTTP/1.1
                string[] requestArr;
                requestArr = requestLine.Split(' ');
                // length has to be 3
                if (requestArr.Length != 3)
                {
                    IsValid = false;
                } else
                {
                    Method = requestArr[0].ToUpper();
                    Url = new Url(requestArr[1]);
                    // needs linq
                    if (AllowedMethods.Any(Method.Contains))
                    {
                        IsValid = true;
                    }
                }
            }
        }

        public bool IsValid { get; private set; }

        public string Method { get; private set; }

        public IUrl Url { get; private set; }

        public IDictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();

        public string UserAgent { get; private set; }

        public int HeaderCount => Headers.Count;

        public int ContentLength { get; private set; }

        public string ContentType { get; private set; }

        public Stream ContentStream => throw new NotImplementedException();

        public string ContentString => throw new NotImplementedException();

        public byte[] ContentBytes => throw new NotImplementedException();
    }
}
