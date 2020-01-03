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
            ReqStreamReader = new StreamReader(requestStream, leaveOpen: true);
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
            // get Headers
            while(!ReqStreamReader.EndOfStream)
            {
                string streamLine = ReqStreamReader.ReadLine();

                // when the header is empty -> content will be read
                if (streamLine == "")
                {
                    // break if there is no content
                    if (ContentLength == 0) break;

                    var charContent = new char[ContentLength];
                    ReqStreamReader.Read(charContent, 0, ContentLength);
                    Content = new string(charContent);
                    break;
                }

                // split the stringstream and extract all headers
                string[] headerLine = streamLine.Split(": ");

                if (headerLine.Length == 2)
                {
                    switch (headerLine[0].ToLower())
                    {
                        case "content-type":
                            ContentType = headerLine[1];
                            break;
                        case "user-agent":
                            UserAgent = headerLine[1];
                            break;
                        case "content-length":
                            ContentLength = Int16.Parse(headerLine[1]); // convert string to int (short)
                            break;
                    }
                    Headers.Add(headerLine[0].ToLower(), headerLine[1]);
                }
                // maybe throw an exception?
            }
            // stream not writable?
            ReqStreamReader.Close();

        }

        public bool IsValid { get; private set; }

        public string Method { get; private set; }

        public IUrl Url { get; private set; }

        public IDictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();

        public string UserAgent { get; private set; }

        public int HeaderCount => Headers.Count;

        public int ContentLength { get; private set; }

        public string ContentType { get; private set; }

        private string Content { get; set; } = "";

        public Stream ContentStream
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                StreamWriter sw = new StreamWriter(ms, Encoding.ASCII);

                sw.Write(Content);

                sw.Flush();

                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }
        }
        

        public string ContentString => Content;

        public byte[] ContentBytes => Encoding.UTF8.GetBytes(Content);
    }
}
