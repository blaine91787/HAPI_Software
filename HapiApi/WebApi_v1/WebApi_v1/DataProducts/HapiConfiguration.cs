using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using WebApi_v1.DataProducts.RBSpiceA;

namespace WebApi_v1.DataProducts
{
    public static class Hapi
    {
        #region Properties
        private static readonly char[] _delimiters = new char[] { '?', '&', '=' };
        private static readonly string[] _requesttypes = new string[] {
            "data",
            "info",
            "capabilities",
            "catalog"
        };
        private static bool _initialized = false;
        public static bool Initialized { get; private set; }
        private static Dictionary<string, string> QueryDict { get; set; }
        public static HttpRequestMessage Request { get; set; }
        public static HttpResponseMessage Response { get; private set; }
        public static string RequestType { get; set; }
        public static string Query { get; set; }
        public static IProperties Properties { get; set; }
        public static List<Exception> Errors { get; private set; }
        public static IProduct Product { get; private set; }
        #endregion

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
                default:
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
                Errors.Add(e); // Ask about whether I should actually be saving the errors.
                return false;
            }

            return true;
        } 

        public static bool CreateResponse ()
        {
            switch (RequestType.ToLower())
            {
                case ("data"):
                    try
                    {
                        GetData();
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Errors.Add(e);
                        return false;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(RequestType, "Not a valid request type.");
            }

            //Response = new HapiResponse();
            Response = Request.CreateResponse(HttpStatusCode.Accepted, Product);

            return true;
        }
        
        private static void GetData()
        {
            string id = Properties.Id;
            switch (id)
            {
                case ("rbspicea_l0_aux"):
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
            if (!_requesttypes.Contains(RequestType))
                return false;

            return true;
        }

        public static Dictionary<string,string> GetDictionaryFromQuery(string query)
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
                    RequestType.ToString(),
                    Query.ToString(),
                    props
                );
                return str;
            }
        }
        #endregion
    }

    //public abstract class Properties : IProperties
    //{
    //    #region Properties
    //    public string RequestType { get; set; }
    //    public string Id { get; set; }
    //    public DateTime TimeMin { get; set; }
    //    public DateTime TimeMax { get; set; }
    //    public List<string> Parameters { get; set; }
    //    public bool IncludeHeader { get; set; }
    //    public Exception Error { get; set; }
    //    #endregion

    //    #region Methods
    //    public abstract void Assign(Dictionary<string, string> dict);
    //    #endregion
    //}

    public class DataProperties : IProperties//Properties, IProperties
    {
        #region Properties
        public string RequestType { get; set; }
        public string Id { get; set; }
        public DateTime TimeMin { get; set; }
        public DateTime TimeMax { get; set; }
        public List<string> Parameters { get; set; }
        public bool IncludeHeader { get; set; }
        public Exception Error { get; set; }
        #endregion

        #region Methods
        public void Assign(Dictionary<string,string> dict)
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
                        break;
                    case ("time.min"):
                        dt = Convert.ToDateTime(val);
                        if (dt != default(DateTime))
                            TimeMin = dt;
                        break;
                    case ("time.max"):
                        dt = Convert.ToDateTime(val);
                        if (dt != default(DateTime))
                            TimeMax = dt;
                        break;
                    case ("parameters"):
                        Parameters = new List<string>();
                        Parameters = val.Split(new char[] { ',' }).ToList();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(key, String.Format("The url parameter '{0}={1}' is not valid.",key,val));
                }
            }
        }
        public override string ToString()
        {
            string str = String.Empty;

            string pars = "";
            if (Parameters != null)
                foreach (string par in Parameters)
                    pars += par + ", ";
            else
                pars += "No parameters provided.";

            str = String.Format(
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

            return str;
        }
        #endregion
    }
}