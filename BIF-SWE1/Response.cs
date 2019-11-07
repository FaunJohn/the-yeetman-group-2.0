using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BIF.SWE1.Interfaces;

namespace BIF_SWE1.Uebungen
{
    class Response : IResponse
    {
        public IDictionary<string, string> Headers => throw new NotImplementedException();

        public int ContentLength => throw new NotImplementedException();

        public string ContentType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int StatusCode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string Status => throw new NotImplementedException();

        public string ServerHeader { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void AddHeader(string header, string value)
        {
            throw new NotImplementedException();
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
