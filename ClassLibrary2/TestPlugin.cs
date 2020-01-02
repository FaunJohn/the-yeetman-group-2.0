using System;
using System.Linq;
using BIF.SWE1.Interfaces;
using BIF_SWE1;

namespace TestPlugin
{
    // Todo: Rework?
    public class TestPlugin : IPlugin, IPluginName
    {
        public string Name { get; } = "TestPlugin";

        public float CanHandle(IRequest req)
        {
            if (req == null)
                return 0.0f;

            if (req.IsValid && (req.Url.Segments[0] == "test" ||
                                req.Url.Parameter.ContainsKey("test_plugin")))
            {
                return 1.0f;
            }
            else if (req.IsValid && req.Url.RawUrl == "/")
            {
                return 0.1f;
            }

            return 0.0f;
        }

        public IResponse Handle(IRequest req)
        {
            if (req == null)
                return new BIF_SWE1.Uebungen.Response { StatusCode = 404 };

            if (req.IsValid && (req.Url.Segments[0] == "test" ||
                                req.Url.Parameter.ContainsKey("test_plugin")))
            {
                BIF_SWE1.Uebungen.Response resp = new BIF_SWE1.Uebungen.Response { StatusCode = 200 };
                resp.SetContent("Empty Response.");

                return resp;
            }
            else if (req.IsValid && req.Url.RawUrl == "/")
            {
                BIF_SWE1.Uebungen.Response resp = new BIF_SWE1.Uebungen.Response { StatusCode = 200 };
                resp.SetContent("Valid request, TestPlugin has no way to handle this request.");

                return resp;
            }
            else
            {
                BIF_SWE1.Uebungen.Response resp = new BIF_SWE1.Uebungen.Response { StatusCode = 500 };

                return resp;
            }
        }
    }
}
