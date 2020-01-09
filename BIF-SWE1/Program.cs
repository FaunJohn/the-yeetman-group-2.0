using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using BIF_SWE1.Uebungen;


namespace BIF_SWE1
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Listen();
        }
    }
}
