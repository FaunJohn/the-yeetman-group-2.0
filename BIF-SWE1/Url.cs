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
                if (path.StartsWith("/"))
                {
                    string tempPath = path.Substring(1);
                    Segments = tempPath.Split("/");
                } else
                {
                    Segments = path.Split("/");
                }
                
                string[] fragment = path.Split("#");
                string[] temp = path.Split("?");
                Path = temp[0];

                if(temp.Length !=1)
                {
                    string[] tempVarTwo = temp[1].Split("&");

                    foreach (var x in tempVarTwo)
                    {
                        string[] help = x.Split("=");
                        Parameter[help[0]] = help[1];
                        ParameterCount++;
                    }
                }

                // Fragment implementation
                if(fragment.Length != 1)
                {
                    Path = fragment[0];
                    Fragment = fragment[1];
                }
            }
        }

        public string RawUrl { get; private set; }

        public string Path { get; private set; }

        public IDictionary<string, string> Parameter { get; private set; }

        public string[] Segments { get; private set; }

        public string FileName => throw new NotImplementedException();

        public string Extension => throw new NotImplementedException();

        public string Fragment { get; private set; }

        public int ParameterCount { get; private set; }
    }
}
