using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using WebApi_v1.DataProducts.RBSpiceA;
using WebApi_v1.DataProducts.Utilities;

namespace WebApi_v1.DataProducts
{
    public class HapiConfiguration
    {
        #region ReadOnly Properties

        private readonly string _version = "2.0";
        private readonly string[] _capabilities = { "csv", "json" };
        private readonly char[] _delimiters = new char[] { '?', '&', '=' };
        private readonly string[] _requesttypes = new string[] { "data", "info", "capabilities", "catalog" };

        #endregion ReadOnly Properties

        #region Public Properties

        public string Version { get { return _version; } }
        public string[] Capabilities { get { return _capabilities; } }
        public string[] Formats { get { return _capabilities; } }
        public bool Initialized { get; private set; }
        public Dictionary<string, string> QueryDict { get; private set; }
        public HttpRequestMessage Request { get; set; }
        public HttpResponseMessage Response { get; private set; }
        public IProperties Properties { get; set; }
        public string RequestType { get; set; }
        public string Query { get; set; }
        public List<Exception> Errors { get; private set; }
        public IProduct Product { get; private set; }

        #endregion Public Properties

        #region Methods

        public void Initialize()
        {
            QueryDict = null;
            Request = null;
            Response = null;
            RequestType = String.Empty;
            Query = String.Empty;
            Properties = null;
            Errors = new List<Exception>();
            Product = null;
            Initialized = true;
        }

        public bool Configure(HttpRequestMessage request)
        {
            Initialize();
            Request = request;

            if (!RequestTypeValid())
                return false;

            //Request type is valid.

            if (!TryDictionaryFromQuery())
                return false;


            Properties = new HapiProperties();
            // Choose the correct Properties type based on RequestType
            //switch (RequestType)
            //{
            //    case "data":
            //        Properties = new HapiProperties();
            //        break;

            //    case "info":
            //        Properties = new HapiProperties();
            //        break;

            //    case "catalog":
            //        Properties = new HapiProperties();
            //        return true;
            //}

            // Try to assign query arguments to properties object.
            // Invalid arguments will cause exception.
            try
            {
                Properties.Assign(Formats, QueryDict);
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

        public bool CreateResponse()
        {
            IResponse resp;
            switch (RequestType.ToLower())
            {
                case ("data"):
                    GetDataProduct();
                    try { resp = new DataResponse(this); }
                    catch (Exception e) { Errors.Add(e); return false; }
                    Response = Request.CreateResponse(HttpStatusCode.Accepted);
                    string format = Properties.Format != null ? Properties.Format : "csv";
                    string strcontent = resp.GetResponse(format);
                    Response.Content = new StringContent(strcontent);
                    break;

                case ("catalog"):
                    resp = new CatalogResponse(this);
                    Response = Request.CreateResponse(HttpStatusCode.Accepted);
                    Response.Content = new StringContent(resp.GetResponse());
                    break;

                case ("info"):
                    resp = new InfoResponse(this);
                    Response = Request.CreateResponse(HttpStatusCode.Accepted);
                    Response.Content = new StringContent(resp.GetResponse());
                    break;

                case ("capabilities"):
                    resp = new CapabilitiesResponse(this);
                    Response = Request.CreateResponse(HttpStatusCode.Accepted);
                    Response.Content = new StringContent(resp.GetResponse());
                    break;

                default:
                    Response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to create " + RequestType.ToLower() + " product due to internal error.");
                    throw new ArgumentOutOfRangeException(RequestType, "Not a valid request type.");
            };
            return true;
        }

        private void GetDataProduct()
        {
            switch (Properties.SC)
            {
                case ("rbspicea"):
                    Product = new RBSpiceAProduct(this.Properties);
                    Product.GetProduct();
                    return;

                default:
                    throw new ArgumentOutOfRangeException(Properties.Id, "Not a valid spacecraft ID.");
            }
            //Product = new RBSpiceAProduct();
        }

        private bool RequestTypeValid()
        {
            RequestType = Request.RequestUri.LocalPath.Split('/').Last().ToLower();
            Query = Request.RequestUri.Query;

            // RequestType can only be equal to info, capability, catalog, or data
            return _requesttypes.Contains(RequestType);
        }

        public bool TryDictionaryFromQuery()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string query = Request.RequestUri.Query;

            string[] arr = query.ToLower().TrimStart(_delimiters).Split(_delimiters);

            if (arr.Length >= 2 && arr.Length % 2 == 0)
            {
                for (int i = 0; i < arr.Length; i += 2)
                    dict.Add(arr[i], arr[i + 1]);
            }
            else
            {
                return false;
            }

            QueryDict = dict;

            return true;
        }

        new public string ToString
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

        #region Helper Classes

        public class HapiProperties : IProperties
        {
            #region Properties

            private enum IndexOf
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
            public string Format { get; set; }
            public Exception Error { get; set; }

            #endregion Properties

            #region Methods

            public void Initialize()
            {
                RequestType = "";
                Id = "";
                SC = "";
                Level = "";
                RecordType = "";
                TimeMin = default(DateTime);
                TimeMax = default(DateTime);
                Parameters = new List<string>();
                IncludeHeader = false;
                Format = "csv";
                Error = null;
            }

            public void Assign(string[] formats, Dictionary<string, string> dict)
            {
                Initialize();
                string key = String.Empty;
                string val = String.Empty;
                DateTime dt = default(DateTime);
                foreach (KeyValuePair<string, string> pair in dict)
                {
                    // TODO: Find a better way to check for 'HapiResponse.ToJson>string last' error where time.min and time.max are not valid. Should be able to catch it here.
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
                            dt = Converters.ConvertHapiYMDToDateTime(val);
                            if (dt != default(DateTime))
                                TimeMin = dt.ToUniversalTime();
                            break;

                        case ("time.max"):
                            dt = Converters.ConvertHapiYMDToDateTime(val);
                            if (dt != default(DateTime))
                                TimeMax = dt.ToUniversalTime();
                            break;

                        case ("parameters"):
                            Parameters = val.Split(new char[] { ',' }).ToList();
                            break;

                        case ("include"):
                            if (val == "header")
                                IncludeHeader = true;
                            else
                                throw new ArgumentOutOfRangeException(key, "Include only has one possible value \"include=header\"");
                            break;

                        case ("format"):
                            if (formats.Contains(val))
                                Format = val;
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

        private class CapabilitiesResponse : IResponse
        {
            public string HapiVersion { get; private set; }
            public Status Status { get; }
            public string[] OutputFormats { get; }

            public CapabilitiesResponse(HapiConfiguration hapi)
            {
                HapiVersion = hapi.Version;
                Status = new Status(1200, "OK");
                OutputFormats = hapi.Capabilities;
            }

            public string GetResponse(string format)
            {
                return "";
            }

            public string GetResponse()
            {
                StringBuilder sb = new StringBuilder();
                return sb.Append(String.Format(
                    "{{\n" +
                    "\t\"{0}\" : \"{1}\",\n" +
                    "\t\"{2}\" : {{ \"code\" : {3}, \"message\" : \"{4}\" }},\n" +
                    "\t\"{5}\" : [ \"{6}\", \"{7}\" ]\n" +
                    "}}\n",
                    "Hapi",
                    HapiVersion,
                    "status",
                    Status.Code,
                    Status.Message,
                    "outputFormats",
                    OutputFormats[0],
                    OutputFormats[1]
                )).ToString();
            }
        }

        private class DataResponse : IResponse
        {
            private HapiConfiguration Hapi;
            public string HapiVersion;
            public Status Status = null;
            public string StartDate = String.Empty;
            public string StopDate = String.Empty;
            public List<string> Parameters = null;
            public string Format = null;
            public IEnumerable<Dictionary<string, string>> Data = null;

            public DataResponse(HapiConfiguration hapi)
            {
                Hapi = hapi;
                IProperties props = Hapi.Properties;
                if (props == null)
                    throw new MissingFieldException(nameof(hapi.Properties));

                if (props.TimeMin == null)
                    throw new MissingFieldException(nameof(props.TimeMin));

                if (props.TimeMax == null)
                    throw new MissingFieldException(nameof(props.TimeMax));

                if (Hapi.Product == null)
                    throw new MissingFieldException(nameof(Hapi.Product));

                HapiVersion = Hapi.Version;
                StartDate = props.TimeMin.ToString();
                StopDate = props.TimeMax.ToString();
                Parameters = props.Parameters;
                Format = props.Format;
                Status = new Status(1200, "OK");
                Data = hapi.Product.Records;
            }

            public string GetResponse()
            {
                return "";
            }

            public string GetResponse(string format)
            {
                string resp = "";
                switch (format.ToLower())
                {
                    case ("csv"):
                        resp = ToCSV();
                        return resp;

                    case ("json"):
                        resp = ToJson();
                        return resp;

                    default:
                        return resp;
                }
            }

            private string GetInfoHeader()
            {
                // TODO: Make GetInfoHeader work for both csv and json versions. Possibly make it it's own helper class.
                // This would allow it to be used for the other responses, not just DataResponse.
                // Also, figure out what to do about multiline parameters.
                bool multiLineParameters = false;
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(
                    "#{{\n" +
                    "#\t\"HAPI\" : \"{0}\",\n" +
                    "#\t\"status\" : {{ \"code\" : {1}, \"message\" : \"{2}\" }},\n" +
                    "#\t\"startDate\" : \"{3}\",\n" +
                    "#\t\"stopDate\" : \"{4}\",\n" +
                    "#\t\"parameters\" : [\n",
                    HapiVersion,
                    Status.Code,
                    Status.Message,
                    Hapi.Properties.TimeMin,
                    Hapi.Properties.TimeMax
                );

                if (Hapi.Properties.Parameters != null)
                {
                    foreach (string param in Hapi.Properties.Parameters)
                    {
                        if (multiLineParameters)
                        {
                            sb.AppendFormat(
                                "#\t\t{{\n " +
                                "#\t\t   \"name\" : \"{0}\",\n" +
                                "#\t\t   \"type\" : \"{1}\",\n" +
                                "#\t\t   \"units\" : \"{2}\",\n" +
                                "#\t\t   \"fill\" : \"{3}\",\n" +
                                "#\t\t   \"length\" : {4},\n" +
                                "#\t\t}},\n",
                                param.ToLower(),
                                "null",
                                "null",
                                "null",
                                "null"
                            );
                        }
                        else
                        {
                            sb.AppendFormat(
                                "#\t\t{{ \"name\" : \"{0}\", \"type\" : \"{1}\", \"units\" : \"{2}\", \"fill\" : \"{3}\", \"length\" : {4} }},\n",
                                param.ToLower(),
                                "null",
                                "null",
                                "null",
                                "null"
                            );
                        }
                    }
                }

                sb.AppendFormat(
                    "#\t],\n#\t\"format\" : \"{0}\",\n#}}\n",
                    Format
                );

                return sb.ToString();
            }

            public string ToCSV()
            {
                bool header = Hapi.Properties.IncludeHeader;
                StringBuilder sb = new StringBuilder();

                if (header)
                {
                    sb.Append(GetInfoHeader());
                }

                string last;
                if (Hapi.Product.Records.Count() > 0)
                {
                    last = Hapi.Product.Records.ToList().First().ToList().Last().Key;
                }
                else
                {
                    return ("No records were found. If this is an error, make sure query is valid.");
                }

                foreach (Dictionary<string, string> rec in Hapi.Product.Records)
                {
                    foreach (KeyValuePair<string, string> pair in rec)
                    {
                        sb.Append(pair.Value);
                        if (!string.Equals(pair.Key, last, StringComparison.OrdinalIgnoreCase))
                            sb.Append(",");
                        else
                            sb.Append("\n");
                    }
                }
                return sb.ToString();
            }

            public string ToJson()
            {
                bool multiLineParameters = false;
                bool header = true;//Hapi.Properties.IncludeHeader;
                StringBuilder sb = new StringBuilder();

                if (header)
                {
                    sb.AppendFormat(
                        "{{\n" +
                        "\t\"HAPI\" : \"{0}\",\n" +
                        "\t\"status\" : {{ \"code\" : {1}, \"message\" : \"{2}\" }},\n" +
                        "\t\"startDate\" : \"{3}\",\n" +
                        "\t\"stopDate\" : \"{4}\",\n" +
                        "\t\"parameters\" : [\n",
                        HapiVersion,
                        Status.Code,
                        Status.Message,
                        Hapi.Properties.TimeMin,
                        Hapi.Properties.TimeMax
                    );

                    if (Hapi.Properties.Parameters != null)
                    {
                        foreach (string param in Hapi.Properties.Parameters)
                        {
                            if (multiLineParameters)
                            {
                                sb.AppendFormat(
                                    "\t\t{{\n " +
                                    "\t\t   \"name\" : \"{0}\",\n" +
                                    "\t\t   \"type\" : \"{1}\",\n" +
                                    "\t\t   \"units\" : \"{2}\",\n" +
                                    "\t\t   \"fill\" : \"{3}\",\n" +
                                    "\t\t   \"length\" : {4},\n" +
                                    "\t\t}},\n",
                                    param,
                                    "null",
                                    "null",
                                    "null",
                                    "null"
                                );
                            }
                            else
                            {
                                sb.AppendFormat(
                                    "\t\t{{ \"name\" : \"{0}\", \"type\" : \"{1}\", \"units\" : \"{2}\", \"fill\" : \"{3}\", \"length\" : {4} }},\n",
                                    param,
                                    "null",
                                    "null",
                                    "null",
                                    "null"
                                );
                            }
                        }
                    }

                    sb.Append("\t],\n"); // trailing ']' for parameters

                    sb.AppendFormat(
                        "\t\"format\" : \"{0}\",\n",
                        Format
                    );

                    sb.Append("\t\"data\" : [\n");
                }

                string last;
                try
                {
                    last = Hapi.Product.Records.ToList().First().ToList().Last().Key;
                }
                catch
                {
                    throw new InvalidOperationException("\"HapiResponse.ToJson()>string last\" is empty. Possibly missing time.min or time.max or invalid request.");
                }

                foreach (Dictionary<string, string> rec in Hapi.Product.Records)
                {
                    if (header)
                        sb.Append("\t\t[");

                    //KeyValuePair<string, string>[] dataArr = rec.ToArray(); ;

                    foreach (KeyValuePair<string, string> pair in rec)
                    {
                        sb.Append(pair.Value);
                        if (pair.Key != last)
                            sb.Append(",");
                    }

                    if (header)
                        sb.Append("],\n");
                    else
                        sb.Append("\n");
                }

                sb.Append("\t]\n}");

                return sb.ToString();
            }
        }

        public class InfoResponse : IResponse
        {
            private HapiConfiguration Hapi;
            private string HapiVersion { get; set; }
            private Status Status { get; set; }
            private string Format { get; set; }
            private DateTime StartDate { get; set; }
            private DateTime StopDate { get; set; }
            private List<string> Parameters { get; set; }

            public InfoResponse(HapiConfiguration hapi)
            {
                Hapi = hapi;
                HapiVersion = hapi.Version;
                Status = new Status(1200, "OK"); // HACK: don't use a literal value for code
                Format = Hapi.Properties.Format == null ? "csv" : Hapi.Properties.Format;
                StartDate = Hapi.Properties.TimeMin;
                StopDate = Hapi.Properties.TimeMax;
                Parameters = Hapi.Properties.Parameters;
            }

            public string GetResponse(string format)
            {
                return "";
            }

            public string GetResponse()
            {
                return GetInfoHeader();
            }

            private string GetInfoHeader()
            {
                bool multiLineParameters = false;
                StringBuilder sb = new StringBuilder();
                sb.Append(String.Format(
                    "{{\n" +
                    "\t\"HAPI\" : \"{0}\",\n" +
                    "\t\"status\" : {{ \"code\" : {1}, \"message\" : \"{2}\" }},\n" +
                    "\t\"startDate\" : \"{3}\",\n" +
                    "\t\"stopDate\" : \"{4}\",\n" +
                    "\t\"parameters\" : [\n",
                    HapiVersion,
                    Status.Code,
                    Status.Message,
                    StartDate,
                    StopDate
                ));

                if (Hapi.Properties.Parameters != null)
                {
                    foreach (string param in Hapi.Properties.Parameters)
                    {
                        if (multiLineParameters)
                        {
                            sb.Append(String.Format(
                                "\t\t{{\n " +
                                "\t\t   \"name\" : \"{0}\",\n" +
                                "\t\t   \"type\" : \"{1}\",\n" +
                                "\t\t   \"units\" : \"{2}\",\n" +
                                "\t\t   \"fill\" : \"{3}\",\n" +
                                "\t\t   \"length\" : {4},\n" +
                                "\t\t}},\n",
                                param.ToLower(),
                                "null",
                                "null",
                                "null",
                                "null"
                            ));
                        }
                        else
                        {
                            sb.Append(String.Format(
                                "\t\t{{ \"name\" : \"{0}\", \"type\" : \"{1}\", \"units\" : \"{2}\", \"fill\" : \"{3}\", \"length\" : {4} }},\n",
                                param.ToLower(),
                                "null",
                                "null",
                                "null",
                                "null"
                            ));
                        }
                    }
                }

                sb.Append("\t],\n}\n");

                return sb.ToString();
            }
        }

        public class CatalogResponse : IResponse
        {
            private HapiConfiguration Hapi;
            private string HapiVersion { get; set; }
            private Status Status { get; set; }
            private List<KeyValuePair<string, string>> Catalog { get; set; }

            public CatalogResponse(HapiConfiguration hapi)
            {
                Hapi = hapi;
                HapiVersion = Hapi.Version;
                Status = new Status(1200, "OK"); // HACK: don't use a literal value for code
                Catalog = new List<KeyValuePair<string, string>>()
                {
                    GetKeyValPair("RBSPICEA_L0_AUX", "RBSPA Level 0 Auxiliary Data"),
                };
            }

            private KeyValuePair<string, string> GetKeyValPair(string id, string title)
            {
                return new KeyValuePair<string, string>(id, title);
            }

            public string GetResponse()
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(
                    String.Format(
                        "{{\n" +
                        "\t\"HAPI\" : \"{0}\",\n" +
                        "\t\"status\" : {{ \"code\" : {1}, \"message\" : \"{2}\" }},\n" +
                        "\t\"catalog\" :\n" +
                        "\t[\n",
                        HapiVersion,
                        Status.Code,
                        Status.Message
                    )
                );

                foreach (var dict in Catalog)
                {
                    sb.Append(
                        String.Format(
                            "\t\t{{ \"id\" : \"{0}\", \"title\" : \"{1}\" }},\n",
                            dict.Key,
                            dict.Value
                        )
                    );
                }

                sb.Append("\t]\n}}");

                return sb.ToString();
            }

            public string GetResponse(string format)
            {
                return "";
            }
        }

        #endregion Helper Classes

        #region Internal Classes

        internal interface IResponse
        {
            string GetResponse();

            string GetResponse(string format);
        }

        internal class Status
        {
            public int Code = -1;
            public string Message = null;

            public Status(int code, string message)
            {
                Code = code;
                Message = message;
            }
        }

        #endregion Internal Classes
    }
}