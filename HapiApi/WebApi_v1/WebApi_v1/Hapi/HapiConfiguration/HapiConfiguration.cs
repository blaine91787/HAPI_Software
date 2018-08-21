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
        private string[] _validIDs = new string[] { "rbspicea" };

        #endregion Private Properties

        #region Public Properties

        public string Basepath { get { return _basepath; } }
        public string DataArchivePath { get { return _dataArchivePath; } }
        public string Version { get { return _version; } }
        public string RequestType { get; private set; }
        public string Query { get; private set; }
        public string[] Capabilities { get { return _capabilities; } }
        public string[] Formats { get { return _capabilities; } }
        public string[] ValidIDs { get { return _validIDs; } }
        public HttpRequestMessage Request { get; private set; }
        public HttpResponseMessage Response { get; private set; }
        public HapiDataProduct Product { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public void ParseRequest(HttpRequestMessage request)
        {
            // Get Hapi Specification defined defaults.
            HapiXmlReader hxr = new HapiXmlReader();
            hxr.LoadHapiSpecs(out _version, out _capabilities, out _, out _dataArchivePath, out _);

            HapiPaths paths = new HapiPaths();
            paths.ResolveUserPath();
            _basepath = paths.DataPath;



            // TODO: Need to integrate the HapiCatalog
        }


        #endregion Private Methods
    }
}