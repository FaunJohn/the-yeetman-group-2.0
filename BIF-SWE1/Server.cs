using BIF.SWE1.Interfaces;
using BIF_SWE1.Uebungen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BIF_SWE1
{
    class Server
    {
        public int Port = 8080;

        private PluginManager PluginManager { get; set; } = new PluginManager();

        public void Listen()
        {
            Console.WriteLine("Starting Server...");
            TcpListener tcpListener = new TcpListener(IPAddress.Any, Port);
            Console.WriteLine("Listening on Port " + Port);
            tcpListener.Start();

            while(true)
            {
                Socket s = tcpListener.AcceptSocket();
                Thread thread = new Thread(()=> ProcessRequest(s));
                thread.Start();
            }
        }

        private void ProcessRequest(Socket s)
        {
            Stream stream = new NetworkStream(s);
            Request request = new Request(stream);
            Response response = null;
            IPlugin selectedPlugin = null;
            float maxScore = 0.0f;
            string message = "";

            foreach (var plugin in PluginManager.Plugins)
            {
                var score = plugin.CanHandle(request);
                if (score > maxScore)
                {
                    maxScore = score;
                    selectedPlugin = plugin;
                }
            }

            message += "Request\n";
            message += "Method: " + request.Method + " Time: " + DateTime.Now + "\n";

            try
            {
                if(selectedPlugin != null)
                {
                    message += "Plugin: " + selectedPlugin.GetType() + "\n";

                    response = selectedPlugin.Handle(request) as Response;
                    if(response != null)
                    {
                        response.Send(stream);
                    }
                    else
                    {
                        response = new Response { StatusCode = 500 };
                        response.SetContent(response.Status);
                        response.Send(stream);
                    }
                }
                else
                {
                    message += "Plugin: No Plugin selected\n";
                    response = new Response { StatusCode = 500 };
                    response.SetContent(response.Status);
                    response.Send(stream);
                }
            }
            catch (Exception ex)
            {
                message += "Error: " + ex.Message + "\n";
                response = new Response { StatusCode = 500 };
                response.SetContent(response.Status);
            }

            message += "Status: " + response.Status + "\n";
            Console.WriteLine(message);

            stream.Close();
        }
    }
}
