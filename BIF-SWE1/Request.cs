using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;


namespace BIF_SWE1.Uebungen
{
    /// <summary>
    /// Manages requests, one at a time.
    /// The class parses information from the request stream.
    /// </summary>
    class Request : IRequest
    {
        /// <summary>
        /// Reads the content of an input stream and calls the ProcessRequest function
        /// </summary>
        /// <param name="requestStream">Content Stream to read from</param>
        public Request(Stream requestStream)
        {
            ReqStreamReader = new StreamReader(requestStream, leaveOpen: true);
            ProcessRequest();
        }

        private StreamReader ReqStreamReader { get; }

        /// <summary>
        /// List of allowed HTTP methods for the server to process
        /// </summary>
        private string[] AllowedMethods { get; } = { "GET", "POST" };

        /// <summary>
        /// Parses the request stream content and saves its information
        /// </summary>
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
                Console.WriteLine(requestLine); // should look like this: GET /favicon.ico HTTP/1.1
                Console.WriteLine("END");

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
            }
            ReqStreamReader.Close();

        }

        /// <summary>
        /// States if a Request is valid. If the method url can be parsed the request is valid.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Returns the requested method (UPPERCASE -> GET or POST)
        /// </summary>
        public string Method { get; private set; }

        /// <summary>
        /// Returns URL object of the request
        /// </summary>
        public IUrl Url { get; private set; }

        /// <summary>
        /// Returns the requested header
        /// </summary>
        public IDictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// Returns the requested UserAgent
        /// </summary>
        public string UserAgent { get; private set; }

        /// <summary>
        /// Returns the HeaderCount, can be 0 if the header is not found
        /// </summary>
        public int HeaderCount => Headers.Count;

        /// <summary>
        /// Returns the parsed ContentLength request header.
        /// </summary>
        public int ContentLength { get; private set; }

        /// <summary>
        /// Returns the parsed ContentType request header.
        /// </summary>
        public string ContentType { get; private set; }

        private string Content { get; set; } = "";

        /// <summary>
        /// Returns the content stream of the request
        /// </summary>
        public Stream ContentStream
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                StreamWriter sw = new StreamWriter(ms, Encoding.ASCII);

                sw.Write(Content);

                // Clears all buffers for the current writer and causes any buffered data to be written to the underlying stream.
                sw.Flush();

                // Set the position to the beginning of the stream.
                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }
        }
        
        /// <summary>
        /// Returns the content of the request as a string
        /// </summary>
        public string ContentString => Content;

        /// <summary>
        /// Returns the content of the request as bytes
        /// </summary>
        public byte[] ContentBytes => Encoding.UTF8.GetBytes(Content);
    }
}
