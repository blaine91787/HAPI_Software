using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using WebApi_v1.HapiDataProducts;
using WebApi_v1.HapiDataProducts.SpaceCraft.RBSpiceA.Products;
using static WebApi_v1.Hapi.HapiResponse;
using static WebApi_v1.Hapi.HapiResponse.Status;

namespace WebApi_v1.Hapi
{
    public class HapiConfiguration
    {
        #region Private Properties

        private readonly string _version = "2.0";
        private readonly string[] _capabilities = { "csv", "json" };
        private readonly char[] _delimiters = new char[] { '?', '&', '=' };
        private readonly string[] _requesttypes = new string[] { "data", "info", "capabilities", "catalog" };
        private readonly string[] _validIDs = new string[] { "rbspicea" };
        private readonly List<Tuple<string, string>> _catalog = new List<Tuple<string, string>>()
        {
            Tuple.Create("RBSPICEA_L0_AUX", "RBSPA Level 0 Auxiliary Data"),
        };

        #endregion Private Properties

        #region Public Properties

        public string Version { get { return _version; } }
        public string RequestType { get; private set; }
        public string Query { get; private set; }
        public string[] Capabilities { get { return _capabilities; } }
        public string[] Formats { get { return _capabilities; } }
        public string[] ValidIDs { get { return _validIDs; } }
        public List<Tuple<string,string>> Catalog { get { return _catalog; } }
        public Dictionary<string, string> QueryDict { get; private set; }
        public HttpRequestMessage Request { get; private set; }
        public HttpResponseMessage Response { get; private set; }
        public HapiProperties Properties { get; private set; }
        public Product Product { get; private set; }

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

            ResponseContent content = null;
            switch (RequestType.ToLower())
            {
                case ("data"):
                    if (Properties.ErrorCodes.Count() == 0 && GetDataProduct())
                        content = ResponseContent.Create(this, "data");
                    break;

                case ("catalog"):;
                    content = ResponseContent.Create(this, "catalog");
                    break;

                case ("info"):
                    content = ResponseContent.Create(this, "info");
                    break;

                case ("capabilities"):
                    content = ResponseContent.Create(this, "capabilities");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(RequestType, "Not a valid request type.");
            };

            if (Properties.ErrorCodes.Count() == 0 && content != null)
            {
                Response = Request.CreateResponse(HttpStatusCode.OK);
                content.SetStatusCode(HapiStatusCode.OK);
                Response.Content = new StringContent(content.GetResponse());
            }
            else
            {
                content = ResponseContent.Create(this, "error");
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
            RequestType = String.Empty;
            Query = String.Empty;
            QueryDict = null;
            Request = null;
            Response = null;
            Properties = new HapiProperties();
            Product = null;
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
                Properties.ErrorCodes.Add(HapiStatusCode.UserInputError);
                return false;
            }

            QueryDict = dict;

            return true;
        }

        private bool GetDataProduct()
        {
            switch (Properties.SC)
            {
                case ("rbspicea"):
                    Product = new RBSpiceAProduct();
                    Product.Configure(this);
                    break;

                default:
                    break;
            }

            if (!Product.VerifyTimeRange()) // Outside of SC data timerange
            {
                Properties.ErrorCodes.Add(HapiStatusCode.TimeOutsideValidRange);
                return false;
            }
            else if (!Product.GetProduct()) // Data doesn't exist
            {
                Properties.ErrorCodes.Add(HapiStatusCode.OKNoDataForTimeRange);
                return false;
            }

            return true;
        }

        #endregion Private Methods
    }
}