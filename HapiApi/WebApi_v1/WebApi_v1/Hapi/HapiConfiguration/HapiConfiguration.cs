using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using WebApi_v1.HAPI.DataProducts;
using WebApi_v1.HAPI.Utilities;
using WebApi_v1.HAPI.Response;
using WebApi_v1.HAPI.Catalog;
using WebApi_v1.HAPI.Properties;

namespace WebApi_v1.HAPI.Configuration
{
    public class HapiConfiguration
    {
        #region Private Properties

        private string _basepath = String.Empty;
        private string _dataArchivePath = String.Empty;
        private string _version = String.Empty;
        private string[] _capabilities = null;

        #endregion Private Properties

        #region Public Properties

        public string Basepath { get { return _basepath; } }
        public string DataArchivePath { get { return _dataArchivePath; } }
        public string Version { get { return _version; } }
        public string RequestType { get; private set; }
        public string Query { get; private set; }
        public string[] Capabilities { get { return _capabilities; } }
        public string[] Formats { get { return _capabilities; } }
        public HapiPaths Paths { get; private set; }
        public HttpRequestMessage Request { get; private set; }
        public HttpResponseMessage Response { get; private set; }
        public HapiDataProduct Product { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public void ParseRequest(HttpRequestMessage request)
        {

            Paths = new HapiPaths();
            Paths.ResolveUserPath();
            _basepath = Paths.DataPath;

            // Get Hapi Specification defined defaults.
            HapiXmlReader hxr = new HapiXmlReader();
            hxr.LoadHapiSpecs(Paths.ConfigurationXmlPath, out _version, out _capabilities, out _, out _dataArchivePath, out _);

            // TODO: Need to integrate the HapiCatalog
        }


        #endregion Private Methods
    }
}