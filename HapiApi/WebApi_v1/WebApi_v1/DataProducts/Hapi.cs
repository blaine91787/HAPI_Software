using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using WebApi_v1.DataProducts.RBSpiceA;

namespace WebApi_v1.DataProducts
{
    public static partial class Hapi
    {
        #region Properties

        private static readonly char[] _delimiters = new char[] { '?', '&', '=' };

        private static readonly string[] _requesttypes = new string[] {
            "data",
            "info",
            "capabilities",
            "catalog"
        };

        public static bool Initialized { get; private set; }
        private static Dictionary<string, string> QueryDict { get; set; }
        public static HttpRequestMessage Request { get; set; }
        public static HttpResponseMessage Response { get; private set; }
        public static string RequestType { get; set; }
        public static string Query { get; set; }
        public static IProperties Properties { get; set; }
        public static List<Exception> Errors { get; private set; }
        public static IProduct Product { get; private set; }

        #endregion Properties

        #region Methods

        public static void Initialize()
        {
            QueryDict = new Dictionary<string, string>();
            Request = null;
            Response = null;
            RequestType = String.Empty;
            Query = String.Empty;
            Properties = null;
            Errors = new List<Exception>();
            Product = null;
            Initialized = true;
        }

        public static bool Configure(HttpRequestMessage request)
        {
            Initialize();
            Request = request;
            RequestType = Request.RequestUri.LocalPath.Split('/').Last().ToLower();

            if (!RequestTypeValid())
                return false;

            //Request type is valid.

            QueryDict = GetDictionaryFromQuery(Query);

            // Choose the correct Properties type based on RequestType
            switch (RequestType)
            {
                case "data":
                    Properties = new DataProperties();
                    break;
            }

            // Try to assign query arguments to properties object.
            // Invalid arguments will cause exception.
            try
            {
                Properties.Assign(QueryDict);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Errors.Add(e); // TODO: Ask about whether I should actually be saving the errors.
                return false;
            }

            if (Properties != null)
                return true; // TODO: Should I return booleans or throw errors?
            else
                return false;
        }

        public static bool CreateResponse()
        {
            switch (RequestType.ToLower())
            {
                case ("data"):
                    GetDataProduct();
                    Hapi.DataResponse resp;
                    try { resp = new Hapi.DataResponse(); }
                    catch (Exception e) { Errors.Add(e); return false; }
                    Response = Request.CreateResponse(HttpStatusCode.Accepted);
                    Response.Content = new StringContent(resp.ToJson());
                    break;

                default:
                    Response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to create " + RequestType.ToLower() + " product due to internal error.");
                    throw new ArgumentOutOfRangeException(RequestType, "Not a valid request type.");
            }

;

            return true;
        }

        private static void GetDataProduct()
        {
            switch (Properties.SC)
            {
                case ("rbspicea"):
                    Product = new RBSpiceAProduct();
                    Product.GetProduct();
                    return;

                default:
                    throw new ArgumentOutOfRangeException(Properties.Id, "Not a valid spacecraft ID.");
            }
            //Product = new RBSpiceAProduct();
        }

        private static bool RequestTypeValid()
        {
            RequestType = Request.RequestUri.LocalPath.Split('/').Last().ToLower();
            Query = Request.RequestUri.Query;

            // RequestType can only be equal to info, capability, catalog, or data
            return _requesttypes.Contains(RequestType);
        }

        public static Dictionary<string, string> GetDictionaryFromQuery(string query)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string[] arr = query.ToLower().TrimStart(_delimiters).Split(_delimiters);

            if (arr.Length >= 2 && arr.Length % 2 == 0)
            {
                for (int i = 0; i < arr.Length; i += 2)
                    dict.Add(arr[i], arr[i + 1]);
            }

            return dict;
        }

        new public static string ToString
        {
            get
            {
                string str = String.Empty;
                string props = Properties.ToString();
                str = String.Format(
                    "\n\n\nRequest: \n{0}\n" +
                    "\nRequestType: {1}\n" +
                    "Query: {2}\n" +
                    "\nProperties: \n{3}\n\n\n",
                    Request.ToString(),
                    RequestType,
                    Query,
                    props
                );
                return str;
            }
        }

        #endregion Methods
    }

    public class DataProperties : IProperties
    {
        #region Properties

        public enum IndexOf
        {
            SC = 0,
            Level = 1,
            RecordType = 2
        }

        public string RequestType { get; set; }
        public string Id { get; set; }
        public string SC { get; set; }
        public string Level { get; set; }
        public string RecordType { get; set; }
        public DateTime TimeMin { get; set; }
        public DateTime TimeMax { get; set; }
        public List<string> Parameters { get; set; }
        public bool IncludeHeader { get; set; }
        public Exception Error { get; set; }

        #endregion Properties

        #region Methods

        public void Assign(Dictionary<string, string> dict)
        {
            string key = String.Empty;
            string val = String.Empty;
            DateTime dt = default(DateTime);
            foreach (KeyValuePair<string, string> pair in dict)
            {
                key = pair.Key.ToLower();
                val = pair.Value.ToLower();
                switch (key)
                {
                    case ("id"):
                        Id = val;
                        if (val.Contains('_')) // ex: id=rbspicea_l0_aux
                        {
                            // HACK: May fail given more spacecraft options.
                            string[] valArr = val.Split('_');
                            SC = valArr[(int)IndexOf.SC];
                            Level = valArr[(int)IndexOf.Level];
                            RecordType = valArr[(int)IndexOf.RecordType];
                        }
                        break;

                    case ("time.min"):
                        // TODO: Verify DateTime is being calculated correctly.
                        if (val.Last() != 'z')
                            val += "z";

                        dt = Convert.ToDateTime(val);
                        if (dt != default(DateTime))
                            TimeMin = dt;
                        break;

                    case ("time.max"):
                        // TODO: Verify DateTime is being calculated correctly (TRAILING Z IS WONKY).
                        if (val.Last() != 'z')
                            val += "z";

                        dt = Convert.ToDateTime(val);

                        if (dt != default(DateTime))
                            TimeMax = dt;
                        break;

                    case ("parameters"):
                        Parameters = new List<string>();
                        Parameters = val.Split(new char[] { ',' }).ToList();
                        break;

                    case ("include"):
                        if (val == "header")
                            IncludeHeader = true;
                        else
                            throw new ArgumentOutOfRangeException(key, "Include only has one possible value \"include=header\"");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(key, String.Format("The url parameter '{0}={1}' is not valid.", key, val));
                }
            }
        }

        public override string ToString()
        {
            string pars = "";
            if (Parameters != null)
            {
                foreach (string par in Parameters)
                    pars += par + ", ";
            }
            else
            {
                pars += "No parameters provided.";
            }

            return String.Format(
                "ID: {0}\n" +
                "TimeMin: {1}\n" +
                "TimeMax: {2}\n" +
                "Parameters: {3}\n" +
                "IncludeHeader: {4}\n",
                Id,
                TimeMin == default(DateTime) ? "No start time provided." : TimeMin.ToString(),
                TimeMax == default(DateTime) ? "No end time provided." : TimeMax.ToString(),
                pars,
                IncludeHeader
            );
        }

        #endregion Methods
    }
}