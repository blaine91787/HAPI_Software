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

        private string[] _capabilities = null;

        #endregion Private Properties

        #region Public Properties

        public string Version { get; private set; }
        public string[] Capabilities { get { return _capabilities; } }
        public string[] Formats { get { return _capabilities; } }
        public HapiPaths Paths { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public void Initialize()
        {
            Paths = new HapiPaths();
            Paths.ResolvePaths();
            Version = Hapi.Registry.Version;

            HapiXmlReader hxr = new HapiXmlReader();
            hxr.LoadHapiSpecs(Paths.ConfigurationXmlPath, out _, out _capabilities, out _, out _, out _);
        }


        #endregion Private Methods
    }
}