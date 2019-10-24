using System;
using System.Collections.Generic;
using System.Text;
using BIF.SWE1.Interfaces;

namespace BIF_SWE1
{
    class Url : IUrl
    {
        public Url(string path)
        {
            Path = path;
            RawUrl = path;

            if (RawUrl == null)
            {
                RawUrl = "/";
            }

            if (Path != null)
            {
                string[] temp = Path.Split("&");
                string[] tempVarTwo = temp[1].Split("?");

                Parameter = new Dictionary<string, string>();

                if (temp.Length >= 2)
                {
                    string[] par = temp[1].Split("=");
                    Parameter[par[0]] = par[1];
                    if(tempVarTwo.Length >= 2)
                    {
                        string[] parTwo = tempVarTwo[1].Split("=");
                        Parameter[parTwo[0]] = parTwo[1];
                    }
                }
            }

        }

        public string RawUrl { get; private set; }

        public string Path { get; private set; }

        public IDictionary<string, string> Parameter { get; private set; }


        public int ParameterCount => throw new NotImplementedException();

        public string[] Segments => throw new NotImplementedException();

        public string FileName => throw new NotImplementedException();

        public string Extension => throw new NotImplementedException();

        public string Fragment => throw new NotImplementedException();
    }
}
