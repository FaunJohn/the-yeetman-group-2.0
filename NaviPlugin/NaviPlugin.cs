using BIF.SWE1.Interfaces;
using BIF_SWE1.Uebungen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Xml;

namespace NaviPlugin
{
    /// <summary>
    /// Navi class plugin
    /// A user enters a street name and the plugin finds all cities in which the street is available
    /// </summary>
    public class NaviPlugin : IPlugin, IPluginName
    {
        private bool mapDataLoadingCheck;
        private readonly Mutex preventFileAccessMutex = new Mutex(); // prevents file access if file is already open
        private readonly Dictionary<string, List<string>> StreetCityList = new Dictionary<string, List<string>>(); // Dictionary with street name (string) and cities where the name occurs

        /// <summary>
        /// Name of the plugin 
        /// </summary>
        public string Name { get; } = "NaviPlugin";

        /// <summary>
        /// Checks if the given Plugin can handle a request
        /// </summary>
        /// <param name="req">Given request</param>
        /// <returns>Float</returns>
        public float CanHandle(IRequest req)
        {
            if (req == null)
                return 0.0f;

            if (req.IsValid && req.Url.Segments[0].ToLower().Equals("navi") || (req.IsValid && req.Url.Segments[0].ToLower() == "navi.html"))
            {
                return 1.0f;
            }

            return 0.0f;
        }

        /// <summary>
        /// Starts loading map data from .osm file if initialized
        /// </summary>
        public NaviPlugin()
        {
            // constructor -> load map data
            StartLoadingMapData();
        }

        /// <summary>
        /// Handles the request 
        /// </summary>
        /// <param name="req"></param>
        /// <returns>Valid response with response code...</returns>
        public IResponse Handle(IRequest req)
        {
            if (req == null) return new Response { StatusCode = 404 };

            if ((req.IsValid && req.Url.Segments.Length == 2 && req.Url.Segments[0].ToLower() == "navi" && req.Url.Segments[1].ToLower() == "reload") || (req.IsValid && req.Url.Segments.Length == 2 && req.Url.Segments[0].ToLower() == "navi.html" && req.Url.Segments[1].ToLower() == "reload"))
            {
                if(mapDataLoadingCheck)
                {
                    // return service unavailable if the server is already loading the map data
                    return new Response { StatusCode = 503 };
                }

                // load map data
                StartLoadingMapData();
                return new Response { StatusCode = 200 };
            }
            // A post request with the navi segment requests map data as json
            else if ((req.IsValid && req.Url.Segments.Length == 1 && req.Url.Segments[0].ToLower() == "navi" && req.Method == "POST") ||(req.IsValid && req.Url.Segments.Length == 1 && req.Url.Segments[0].ToLower() == "navi.html" && req.Method == "POST"))
            {
                return GetNavi(req);
            }
            else
            {
                return new Response { StatusCode = 404};
            }
        }

        /// <summary>
        /// Starts map loading in a seperate thread
        /// </summary>
        public void StartLoadingMapData()
        {
            Thread thread = new Thread(LoadMapData);
            thread.Start();
        }

        /// <summary>
        /// Load data from osm xml file from the path "./navi/austria.osm"
        /// </summary>
        public void LoadMapData()
        {
            XmlReaderSettings settings = new XmlReaderSettings();

            lock(preventFileAccessMutex)
            {
                // set map data loading check to true to inform other treads, that loading has commenced
                mapDataLoadingCheck = true;
                Console.WriteLine("Map data is loading now....");
                var dataFile = "./navi/austria.osm";

                var f = File.OpenRead(dataFile);
                var xml = XmlReader.Create(f, settings);

                while (xml.Read())
                {
                    // check for a xml node named osm
                    if(xml.NodeType == XmlNodeType.Element && xml.Name == "osm")
                    {
                        ReadOsmTree(xml);
                    }
                }
                Console.WriteLine("Map data loading completed!");
                mapDataLoadingCheck = false;
            }
        }

        /// <summary>
        /// Reads an osm xml subtree
        /// </summary>
        /// <param name="xml"></param>
        public void ReadOsmTree(XmlReader xml)
        {
            var osm = xml.ReadSubtree();
            while (osm.Read())
            {
                if (osm.NodeType == XmlNodeType.Element && (osm.Name == "node" || osm.Name == "way"))
                {
                    ReadAnyOsmElement(osm);
                }
            }
        }

        /// <summary>
        /// Reads an osm element into an adress class instance
        /// </summary>
        /// <param name="osm"></param>
        public void ReadAnyOsmElement(XmlReader osm)
        {
            //ReadAnyOsmElement
            Address address = new Address();
            var element = osm.ReadSubtree();

            while (element.Read())
            {
                if (element.NodeType == XmlNodeType.Element && element.Name == "tag")
                {
                    //Read the tags
                    string tagType = element.GetAttribute("k");
                    string value = element.GetAttribute("v");

                    switch (tagType)
                    {
                        case "addr:city":
                            address.City = value;
                            break;
                        case "addr:postcode":
                            address.Zip = value;
                            break;
                        case "addr:street":
                            address.Street = value;
                            break;
                    }
                }

            }

            if (address.Street != null && address.City != null)
            {
                if (StreetCityList.ContainsKey(address.Street))
                {
                    if (!StreetCityList[address.Street].Contains(address.City))
                    {
                        // add city to street
                        StreetCityList[address.Street].Add(address.City);
                    }
                }
                else
                {
                    // add new city list if this is the first city for the street
                    StreetCityList.Add(address.Street, new List<string> { address.City });
                }
            }
        }

        /// <summary>
        /// Creates a response containing the requested map data as json
        /// </summary>
        /// <param name="req">Request</param>
        /// <returns>valid response</returns>
        private Response GetNavi(IRequest req)
        {
            if (mapDataLoadingCheck) // Map data is currently reloading
            {
                return new Response { StatusCode = 503 };
            }

            Response resp = new Response { StatusCode = 200 };
            List<string> cities = new List<string>();
            // Remove any unwanted characters from the search string
            string street = req.ContentString.Trim();
            street = street.Replace("\0", string.Empty);
            if (StreetCityList.ContainsKey(street))
            {
                // Set requested cities that will be returned
                cities = StreetCityList[street];
            }

            try
            {
                // Try to serialize the cities into a json string
                resp.SetContent(JsonSerializer.Serialize(cities));
            }
            catch (Exception ex)
            {
                Response errorResponse = new Response { StatusCode = 500 };
                errorResponse.SetContent("Error serializing cities: " + ex.Message);
                return errorResponse;
            }

            resp.ContentType = resp.KnownFileExtensions["json"] ?? "text/plain";
            return resp;
        }

        /// <summary>
        /// Loads the default map file in a separate thread if flag is set.
        /// Starts processing requests once the map file is loaded.
        /// </summary>
        /// <param name="loadMap"></param>
        public NaviPlugin(bool loadMap = true)
        {
            if (loadMap)
            {
                StartLoadingMapData();
            }
        }

    }
}
