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






        /*
        public static void Read()
        {
            TcpClient s = new TcpClient();
            s.Connect("dasz.at", 80);
            NetworkStream stream = s.GetStream();
            Write(stream);
            StreamReader sr = new StreamReader(stream);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                Console.WriteLine(line);
            }
        }

        public static void Write(NetworkStream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            StreamWriter sw = new StreamWriter(stream);
            sw.WriteLine("GET / HTTP/1.1");
            sw.WriteLine("host: dasz.at");
            sw.WriteLine("connection: close");
            sw.WriteLine();
            sw.Flush();
        }

        private static void Listen()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 8080);
            listener.Start();
            while (true)
            {
                Socket s = listener.AcceptSocket();
                NetworkStream stream = new NetworkStream(s);
                StreamReader sr = new StreamReader(stream);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    Console.WriteLine(line);
                    if (string.IsNullOrEmpty(line)) break;
                }

                StreamWriter sw = new StreamWriter(stream);
                var body = "<html><body><h1>Hello World!</h1><p>a Text</p</body></html>";
                sw.WriteLine("HTTP/1.1 200 OK");
                sw.WriteLine("connection: close");
                sw.WriteLine("content-type: text/html");
                sw.WriteLine("content-length: " + body.Length);
                sw.WriteLine();
                sw.Write(body);
                sw.Write("TESTING");
                sw.Flush();
                s.Close();
            }
        }
        */
    }
}
