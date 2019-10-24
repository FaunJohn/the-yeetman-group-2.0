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
            Parameter = new Dictionary<string, string>();


            if (RawUrl == null)
            {
                RawUrl = "/";
            }

            if (Path != null)
            {
                string[] temp = Path.Split("?");

                if(temp.Length !=1)
                {
                    string[] tempVarTwo = temp[1].Split("&");

                    foreach (var x in tempVarTwo)
                    {
                        string[] help = x.Split("=");
                        Parameter[help[0]] = help[1];
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
