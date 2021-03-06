﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using BIF.SWE1.Interfaces;
using System.IO;

namespace BIF.SWE1.UnitTests
{
    [TestFixture]
    public class OwnTests : AbstractTestFixture<Uebungen.OwnTests>
    {
        private string StaticFileFolder;

        public void SetStaticFileFolder(string folder)
        {
            // do something?
            StaticFileFolder = folder;
        }

        private void SetupStaticFilePlugin(Uebungen.OwnTests ueb, string fileName)
        {
            string folder = Path.Combine(WorkingDirectory, "static-files");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            using (var fs = File.OpenWrite(Path.Combine(folder, fileName)))
            using (var sw = new StreamWriter(fs))
            {
                fs.SetLength(0);
                sw.Write("Testing");
            }
        }



        [Test]
        public void HelloWorld()
        {
            var ueb = CreateInstance();
            ueb.HelloWorld();
        }

        [Test]
        public void navi_can_handle_503()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetNavigationPlugin();
            Assert.That(plugin, Is.Not.Null, "OwnTests.GetNavigationPlugin returned null!");

            var url = ueb.GetNaviUrl();
            Assert.That(url, Is.Not.Null, "IUEB6.GetNaviUrl returned null");

            var req = ueb.GetRequest(RequestHelper.GetValidRequestStream(url, method: "POST", body: "street="));
            Assert.That(req, Is.Not.Null, "OwnTests.GetRequest returned null!");

            Assert.That(plugin.CanHandle(req), Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            var resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(503));
        }

        [Test]
        public void navi_plugin_return_json()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetNavigationPluginWithoutLoadingMap();
            Assert.That(plugin, Is.Not.Null, "IUEB6.GetNavigationPluginWithoutLoadingMap returned null");

            var url = ueb.GetNaviUrl();
            Assert.That(url, Is.Not.Null, "IUEB6.GetNaviUrl returned null");

            var req = ueb.GetRequest(RequestHelper.GetValidRequestStream(url, method: "POST", body: "street=Straße123"));
            Assert.That(req, Is.Not.Null, "IUEB6.GetRequest returned null");

            var score = plugin.CanHandle(req);
            Assert.That(score, Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            var resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(200));
            Assert.That(resp.ContentLength, Is.GreaterThan(0));
            Assert.That(resp.ContentType, Is.EqualTo("text/json"));
        }

        [Test]
        public void navi_plugin_should_not_reload_map_when_busy()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetNavigationPluginWithoutLoadingMap();
            Assert.That(plugin, Is.Not.Null, "IUEB6.GetNavigationPluginWithoutLoadingMap returned null");

            var url = ueb.GetNaviUrl();
            Assert.That(url, Is.Not.Null, "IUEB6.GetNaviUrl returned null");

            var req = ueb.GetRequest(RequestHelper.GetValidRequestStream($"{url}/reload"));
            Assert.That(req, Is.Not.Null, "IUEB6.GetRequest returned null");

            var score = plugin.CanHandle(req);
            Assert.That(score, Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            var resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(200));

            // If we try to reload again we should also get busy response
            resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(503));
        }

        [Test]
        public void lower_plugin_return_not_found()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetToLowerPlugin();
            Assert.That(plugin, Is.Not.Null, "IUEB6.GetToLowerPlugin returned null");

            var url = ueb.GetToLowerUrl();
            Assert.That(url, Is.Not.Null, "IUEB6.GetToLowerUrl returned null");

            var req = ueb.GetRequest(RequestHelper.GetValidRequestStream($"{url}/foo.txt"));
            Assert.That(req, Is.Not.Null, "IUEB6.GetRequest returned null");

            Assert.That(plugin.CanHandle(req), Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            var resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(404));
        }


        [Test]
        public void response_should_return_status_503()
        {
            var obj = CreateInstance().GetResponse();
            Assert.That(obj, Is.Not.Null, "IUEB2.GetResponse returned null");

            obj.StatusCode = 503;
            Assert.That(obj.Status.ToUpper(), Is.EqualTo("503 UNAVAILABLE"));
        }

        [Test]
        public void plugin_manager_check_working_plugins()
        {
            var obj = CreateInstance().GetNavigationPlugin();
            Assert.That(obj, Is.Not.Null, "OwnTests.GetNavigationPlugin returned null");

            var obj_2 = CreateInstance().GetStaticFilePlugin();
            Assert.That(obj_2, Is.Not.Null, "OwnTests.GetStaticFilePlugin returned null");

            var obj_3 = CreateInstance().GetToLowerPlugin();
            Assert.That(obj_3, Is.Not.Null, "OwnTests.GetToLowerPlugin returned null");

        }

        [Test]
        public void staticfileplugin_should_return_html()
        {
            var fileName = string.Format("index.html", Guid.NewGuid());
            var ueb = CreateInstance();
            SetupStaticFilePlugin(ueb, fileName);

            var obj = ueb.GetStaticFilePlugin();
            Assert.That(obj, Is.Not.Null, "IUEB5.GetStaticFilePlugin returned null");

            var url = ueb.GetStaticFileUrl(fileName);
            Assert.That(url, Is.Not.Null, "IUEB5.GetStaticFileUrl returned null");

            var req = ueb.GetRequest(RequestHelper.GetValidRequestStream(url));
            Assert.That(req, Is.Not.Null, "IUEB5.GetRequest returned null");

            Assert.That(obj.CanHandle(req), Is.GreaterThan(0.0f).And.LessThanOrEqualTo(1.0f));
            var resp = obj.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void staticfileplugin_should_return_css()
        {
            var fileName = string.Format("style.css", Guid.NewGuid());
            var ueb = CreateInstance();
            SetupStaticFilePlugin(ueb, fileName);

            var obj = ueb.GetStaticFilePlugin();
            Assert.That(obj, Is.Not.Null, "IUEB5.GetStaticFilePlugin returned null");

            var url = ueb.GetStaticFileUrl(fileName);
            Assert.That(url, Is.Not.Null, "IUEB5.GetStaticFileUrl returned null");

            var req = ueb.GetRequest(RequestHelper.GetValidRequestStream(url));
            Assert.That(req, Is.Not.Null, "IUEB5.GetRequest returned null");

            Assert.That(obj.CanHandle(req), Is.GreaterThan(0.0f).And.LessThanOrEqualTo(1.0f));
            var resp = obj.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void staticfileplugin_should_return_js()
        {
            var fileName = string.Format("script.js", Guid.NewGuid());
            var ueb = CreateInstance();
            SetupStaticFilePlugin(ueb, fileName);

            var obj = ueb.GetStaticFilePlugin();
            Assert.That(obj, Is.Not.Null, "IUEB5.GetStaticFilePlugin returned null");

            var url = ueb.GetStaticFileUrl(fileName);
            Assert.That(url, Is.Not.Null, "IUEB5.GetStaticFileUrl returned null");

            var req = ueb.GetRequest(RequestHelper.GetValidRequestStream(url));
            Assert.That(req, Is.Not.Null, "IUEB5.GetRequest returned null");

            Assert.That(obj.CanHandle(req), Is.GreaterThan(0.0f).And.LessThanOrEqualTo(1.0f));
            var resp = obj.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void navi_plugin_should_load_map_data()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetNavigationPluginWithoutLoadingMap();

            var url = ueb.GetNaviUrl();
            Assert.That(url, Is.Not.Null, "IUEB6.GetNaviUrl returned null");

            var req = ueb.GetRequest(RequestHelper.GetValidRequestStream($"{url}/reload"));
            Assert.That(req, Is.Not.Null, "IUEB6.GetRequest returned null");

            var score = plugin.CanHandle(req);
            Assert.That(score, Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            var resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(200));
        }

        private static StringBuilder GetBody(IResponse resp)
        {
            StringBuilder body = new StringBuilder();
            using (var ms = new MemoryStream())
            {
                resp.Send(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sr = new StreamReader(ms);
                while (!sr.EndOfStream)
                {
                    body.AppendLine(sr.ReadLine());
                }
            }
            return body;
        }

        [Test]
        public void lower_plugin_should_handle_long_string()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetToLowerPlugin();
            Assert.That(plugin, Is.Not.Null, "IUEB6.GetToLowerPlugin returned null");

            var url = ueb.GetToLowerUrl();
            Assert.That(url, Is.Not.Null, "IUEB6.GetToLowerUrl returned null");

            string textToTest = string.Format("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", Guid.NewGuid());

            var req = ueb.GetRequest(RequestHelper.GetValidRequestStream(url, method: "POST", body: string.Format("text={0}", textToTest)));
            Assert.That(req, Is.Not.Null, "IUEB6.GetRequest returned null");

            Assert.That(plugin.CanHandle(req), Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            var resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(200));
            Assert.That(resp.ContentLength, Is.GreaterThan(0));

            StringBuilder body = GetBody(resp);
            Assert.That(body.ToString(), Does.Contain(textToTest.ToLower()));
        }

        [Test]
        public void staticfileplugin_should_return_navi_starting_page()
        {
            var fileName = string.Format("navi.html", Guid.NewGuid());
            var ueb = CreateInstance();
            SetupStaticFilePlugin(ueb, fileName);

            var obj = ueb.GetStaticFilePlugin();
            Assert.That(obj, Is.Not.Null, "IUEB5.GetStaticFilePlugin returned null");

            var url = ueb.GetStaticFileUrl(fileName);
            Assert.That(url, Is.Not.Null, "IUEB5.GetStaticFileUrl returned null");

            var req = ueb.GetRequest(RequestHelper.GetValidRequestStream(url));
            Assert.That(req, Is.Not.Null, "IUEB5.GetRequest returned null");

            Assert.That(obj.CanHandle(req), Is.GreaterThan(0.0f).And.LessThanOrEqualTo(1.0f));
            var resp = obj.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void lower_plugin_handle_empty_custom()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetToLowerPlugin();
            Assert.That(plugin, Is.Not.Null, "IUEB6.GetToLowerPlugin returned null");

            var url = ueb.GetToLowerUrl();
            Assert.That(url, Is.Not.Null, "IUEB6.GetToLowerUrl returned null");

            var req = ueb.GetRequest(RequestHelper.GetValidRequestStream(url, method: "POST", body: "text="));
            Assert.That(req, Is.Not.Null, "IUEB6.GetRequest returned null");

            Assert.That(plugin.CanHandle(req), Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            var resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(200));
            Assert.That(resp.ContentLength, Is.GreaterThan(0));

            StringBuilder body = GetBody(resp);
            Assert.That(body.ToString(), Does.Contain("Bitte geben Sie einen Text ein"));
        }

        [Test]
        public void navi_plugin_handle_non_busy()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetNavigationPluginWithoutLoadingMap();
            Assert.That(plugin, Is.Not.Null, "IUEB6.GetNavigationPluginWithoutLoadingMap returned null");

            var url = ueb.GetNaviUrl();
            Assert.That(url, Is.Not.Null, "IUEB6.GetNaviUrl returned null");

            var req = ueb.GetRequest(RequestHelper.GetValidRequestStream(url, method: "POST", body: "street="));
            Assert.That(req, Is.Not.Null, "IUEB6.GetRequest returned null");

            Assert.That(plugin.CanHandle(req), Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            var resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(200));
            Assert.That(resp.ContentLength, Is.GreaterThan(0));
        }

        [Test]
        public void navi_plugin_should_reload_map()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetNavigationPluginWithoutLoadingMap();
            Assert.That(plugin, Is.Not.Null, "IUEB6.GetNavigationPluginWithoutLoadingMap returned null");

            var url = ueb.GetNaviUrl();
            Assert.That(url, Is.Not.Null, "IUEB6.GetNaviUrl returned null");

            var req = ueb.GetRequest(RequestHelper.GetValidRequestStream($"{url}/reload"));
            Assert.That(req, Is.Not.Null, "IUEB6.GetRequest returned null");

            var score = plugin.CanHandle(req);
            Assert.That(score, Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            var resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(200));

            // Interacting with the plugin should now return busy
            req = ueb.GetRequest(RequestHelper.GetValidRequestStream(url, method: "POST", body: "street=Straße123"));
            score = plugin.CanHandle(req);
            Assert.That(score, Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(503));
        }

        [Test]
        public void staticfileplugin_should_return_lower_starting_page()
        {
            var fileName = string.Format("lower.html", Guid.NewGuid());
            var ueb = CreateInstance();
            SetupStaticFilePlugin(ueb, fileName);

            var obj = ueb.GetStaticFilePlugin();
            Assert.That(obj, Is.Not.Null, "IUEB5.GetStaticFilePlugin returned null");

            var url = ueb.GetStaticFileUrl(fileName);
            Assert.That(url, Is.Not.Null, "IUEB5.GetStaticFileUrl returned null");

            var req = ueb.GetRequest(RequestHelper.GetValidRequestStream(url));
            Assert.That(req, Is.Not.Null, "IUEB5.GetRequest returned null");

            Assert.That(obj.CanHandle(req), Is.GreaterThan(0.0f).And.LessThanOrEqualTo(1.0f));
            var resp = obj.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void plugin_manager_should_return_plugin_dir()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetPluginManager();
            Assert.That(plugin, Is.Not.Null, "IUEB6.GetToLowerPlugin returned null");

            var url = ueb.GetPluginDirectory();
            Assert.That(url, Is.Not.Null, "IUEB6.GetToLowerUrl returned null");
        }

        [Test]
        public void staticfileplugin_throws_exception_on_invalid_file_type()
        {
            var fileName = string.Format("test.jpeg", Guid.NewGuid());
            var ueb = CreateInstance();
            SetupStaticFilePlugin(ueb, fileName);

            var obj = ueb.GetStaticFilePlugin();
            Assert.That(obj, Is.Not.Null, "IUEB5.GetStaticFilePlugin returned null");

            var url = ueb.GetStaticFileUrl("test.osm");
            Assert.That(url, Is.Not.Null, "IUEB5.GetStaticFileUrl returned null");

            var req = ueb.GetRequest(RequestHelper.GetValidRequestStream(url));
            Assert.That(req, Is.Not.Null, "IUEB5.GetRequest returned null");

            if (obj.CanHandle(req) > 0)
            {
                Assert.Throws(Is.InstanceOf<Exception>(), () => { obj.Handle(req); });
            }
        }

        [Test]
        public void navi_plugin_should_return_osm_file_path()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetNavigationPlugin();
            Assert.That(plugin, Is.Not.Null, "OwnTests.GetNavigationPlugin returned null!");

            var url = ueb.GetNaviUrl();
            Assert.That(url, Is.Not.Null, "IUEB6.GetNaviUrl returned null");
        }

        [Test]
        public void navi_plugin_return_not_found()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetNavigationPluginWithoutLoadingMap();
            Assert.That(plugin, Is.Not.Null, "IUEB6.GetNavigationPluginWithoutLoadingMap returned null");

            var url = ueb.GetNaviUrl();
            Assert.That(url, Is.Not.Null, "IUEB6.GetNaviUrl returned null");

            var req = ueb.GetRequest(RequestHelper.GetValidRequestStream($"{url}/foo.txt"));
            Assert.That(req, Is.Not.Null, "IUEB6.GetRequest returned null");

            var score = plugin.CanHandle(req);
            Assert.That(score, Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            var resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(404));
        }


    }
}
