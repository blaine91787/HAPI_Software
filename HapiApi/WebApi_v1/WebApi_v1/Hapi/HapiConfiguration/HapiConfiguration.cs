using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using WebApi_v1.Hapi.DataProducts;
using WebApi_v1.Hapi.Utilities;
using WebApi_v1.Hapi.Response;

namespace WebApi_v1.Hapi
{
    public class Configuration
    {
        #region Private Properties

        private string _basepath = String.Empty;
        private string _dataArchivePath = String.Empty;
        private string _version = String.Empty;
        private string[] _capabilities = null;
        private char[] _delimiters = new char[] { '?', '&', '=' };
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
        public Catalog.Content Catalog { get; private set; }
        public Dictionary<string, string> QueryDict { get; private set; }
        public HttpRequestMessage Request { get; private set; }
        public HttpResponseMessage Response { get; private set; }
        public Properties Properties { get; private set; }
        public DataProduct Product { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public bool Configure(HttpRequestMessage request)
        {
            Initialize();

            if (request == null)
                return false;

            Request = request;
            RequestType = Request.RequestUri.LocalPath.Split('/').Last().ToLower();

            if (!TryToCreateQueryDict())
                return false;

            if (!Properties.Assign(this))
                return false;

            return true;
        }

        public HttpResponseMessage GetResponse()
        {
            if (Properties == null)
                throw new InvalidOperationException("Hapi.Properties not set.");

            Response.Content content = null;
            switch (RequestType.ToLower())
            {
                case ("data"):
                    if (Properties.ErrorCodes.Count() == 0 && GetDataProduct())
                        content = Hapi.Response.Content.Create(this, "data");
                    break;

                case ("catalog"):;
                    content = Hapi.Response.Content.Create(this, "catalog");
                    break;

                case ("info"):
                    content = Hapi.Response.Content.Create(this, "info");
                    break;

                case ("capabilities"):
                    content = Hapi.Response.Content.Create(this, "capabilities");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(RequestType, "Not a valid request type.");
            };

            if (Properties.ErrorCodes.Count() == 0 && content != null)
            {
                Response = Request.CreateResponse(HttpStatusCode.OK);
                content.SetStatusCode(Status.HapiStatusCode.OK);
                Response.Content = new StringContent(content.GetResponse());
            }
            else
            {
                content = Hapi.Response.Content.Create(this, "error");
                Response = Request.CreateResponse(HttpStatusCode.BadRequest);
                content.SetStatusCode(Properties.ErrorCodes.First());
                Response.Content = new StringContent(content.GetResponse());
            }

            return Response;
        }

        #endregion Public Methods

        #region Private Methods

        private void Initialize()
        {
            // Get Hapi Specification defined defaults.
            HapiXmlReader hxr = new HapiXmlReader();
            hxr.LoadHapiSpecs(out _version, out _capabilities, out _, out _dataArchivePath, out _);

            Paths.Resolve();
            _basepath = Paths.DataPath;

            RequestType = String.Empty;
            Query = String.Empty;
            QueryDict = null;
            Request = null;
            Response = null;
            Properties = new Properties();
            Product = null;

            // TODO: Need to integrate the HapiCatalog
            Catalog = new Catalog.Content();
            Catalog.Create();
        }

        private bool TryToCreateQueryDict()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            Query = Request.RequestUri.Query;

            string[] arr = Query.ToLower().TrimStart(_delimiters).Split(_delimiters);

            if (arr.Length >= 2 && arr.Length % 2 == 0)
            {
                for (int i = 0; i < arr.Length; i += 2)
                    dict.Add(arr[i], arr[i + 1]);
            }
            else if (RequestType == "data") // Query empty or 
            {
                Properties.ErrorCodes.Add(Status.HapiStatusCode.UserInputError);
                return false;
            }

            QueryDict = dict;

            return true;
        }

        private bool GetDataProduct()
        {
            Product = DataProduct.Create(this);

            if (!Product.VerifyTimeRange()) // Outside of SC data timerange
            {
                Properties.ErrorCodes.Add(Status.HapiStatusCode.TimeOutsideValidRange);
                return false;
            }
            else if (!Product.GetProduct()) // Data doesn't exist
            {
                Properties.ErrorCodes.Add(Status.HapiStatusCode.OKNoDataForTimeRange);
                return false;
            }

            return true;
        }

        #endregion Private Methods
    }
}