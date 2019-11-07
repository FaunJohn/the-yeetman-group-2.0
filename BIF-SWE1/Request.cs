using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BIF.SWE1.Interfaces;


namespace BIF_SWE1.Uebungen
{
    class Request : IRequest
    {
        public bool IsValid => throw new NotImplementedException();

        public string Method => throw new NotImplementedException();

        public IUrl Url => throw new NotImplementedException();

        public IDictionary<string, string> Headers => throw new NotImplementedException();

        public string UserAgent => throw new NotImplementedException();

        public int HeaderCount => throw new NotImplementedException();

        public int ContentLength => throw new NotImplementedException();

        public string ContentType => throw new NotImplementedException();

        public Stream ContentStream => throw new NotImplementedException();

        public string ContentString => throw new NotImplementedException();

        public byte[] ContentBytes => throw new NotImplementedException();
    }
}
