using System;
using System.Collections.Generic;
using System.Text;
using BIF.SWE1.Interfaces;

namespace BIF_SWE1
{
    /// <summary>
    /// Parses URL information and provides information.
    /// </summary>
    class Url : IUrl
    {
        /// <summary>
        /// Pocesses the path
        /// Provides Information about the url
        /// </summary>
        /// <param name="path">Path name</param>
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

        /// <summary>
        /// Retunrs Raw URL
        /// </summary>
        public string RawUrl { get; private set; }

        /// <summary>
        /// Returns Path as string
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Returns a dictionary with url parameters
        /// </summary>
        public IDictionary<string, string> Parameter { get; private set; }

        /// <summary>
        /// Returns the segments of an url path
        /// </summary>
        public string[] Segments { get; private set; }

        /// <summary>
        /// Returns the filename as string
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// returns the file extension
        /// </summary>
        public string Extension { get; }

        /// <summary>
        /// Returns an url fragment as string
        /// Fragment: Part after # at the end of the url
        /// </summary>
        public string Fragment { get; private set; }

        /// <summary>
        /// Returns the Count of Parameters as int
        /// </summary>
        public int ParameterCount { get; private set; }
    }
}
