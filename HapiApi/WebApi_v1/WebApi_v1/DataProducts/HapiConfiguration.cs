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
        private readonly string[] _validIDs = new string[] { "rbspicea" };


        #endregion ReadOnly Properties

        #region Public Properties

        public string Version { get { return _version; } }
        public string RequestType { get; set; }
        public string Query { get; set; }
        public string[] Capabilities { get { return _capabilities; } }
        public string[] Formats { get { return _capabilities; } }
        public string[] ValidIDs { get { return _validIDs; } }
        public bool Initialized { get; private set; }
        public Dictionary<string, string> QueryDict { get; private set; }
        public HttpRequestMessage Request { get; set; }
        public HttpResponseMessage Response { get; private set; }
        public HapiProperties Properties { get; set; }
        public List<Exception> Errors { get; private set; }
        public Product Product { get; private set; }

        #endregion Public Properties

        #region Methods

        public void Initialize()
        {
            QueryDict = null;
            Request = null;
            Response = null;
            RequestType = String.Empty;
            Query = String.Empty;
            Properties = new HapiProperties();
            Errors = new List<Exception>();
            Product = null;
            Initialized = true;
        }

        public bool Configure(HttpRequestMessage request)
        {
            Initialize();

            Request = request;
            RequestType = Request.RequestUri.LocalPath.Split('/').Last().ToLower();

            if (!TryToCreateQueryDict())
                return false;

            if (!Properties.Assign(this))
                return false;

            return true;
        }

        public void GetErrorResponse()
        {
            ResponseContent content;
            content = new ErrorResponse(this);
            Response = Request.CreateResponse(HttpStatusCode.BadRequest);
            content.SetStatusCode(Properties.ErrorCodes.First());
            Response.Content = new StringContent(content.GetResponse());
        }

        public HttpResponseMessage GetResponse()
        {
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
                content.SetStatusCode(1200);
                Response.Content = new StringContent(content.GetResponse());
            }
            else
            {
                GetErrorResponse();
            }

            return Response;
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

            if (!Product.VerifyTimeRange())
            {
                // Outside of SC data timerange
                Properties.ErrorCodes.Add(1405);
                return false;
            }
            else if(!Product.GetProduct())
            {
                // Data doesn't exist
                Properties.ErrorCodes.Add(1201);
                return false;
            }
            
            return true;
        }

        public bool TryToCreateQueryDict()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            Query = Request.RequestUri.Query;

            string[] arr = Query.ToLower().TrimStart(_delimiters).Split(_delimiters);

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

        public class HapiProperties
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
            public List<int> ErrorCodes { get; set; }
            public bool InTimeRange { get; set; }

            #endregion Properties

            #region Methods

            public HapiProperties()
            {
                Initialize();
            }

            public void Initialize()
            {
                RequestType = String.Empty;
                Id = String.Empty;
                SC = String.Empty;
                Level = String.Empty;
                RecordType = String.Empty;
                TimeMin = default(DateTime);
                TimeMax = default(DateTime);
                Parameters = new List<string>();
                IncludeHeader = false;
                Format = "csv";
                Error = null;
                ErrorCodes = new List<int>();
                InTimeRange = true;
            }

            public bool Assign(HapiConfiguration hapi)
            {
                Initialize();
                Dictionary<string, string> dict = hapi.QueryDict;

                if (!(RequestParametersValid(hapi.RequestType, dict)))
                {
                    ErrorCodes.Add(1401);
                }

                string key = String.Empty;
                string val = String.Empty;
                DateTime dt = default(DateTime);
                Converters cons = new Converters();
                foreach (KeyValuePair<string, string> pair in dict)
                {
                    // TODO: Find a better way to check for 'HapiResponse.ToJson>string last' error where time.min and time.max are not valid. Should be able to catch it here.
                    key = pair.Key.ToLower();
                    val = pair.Value.ToLower();
                    switch (key)
                    {
                        case ("id"):
                            Id = val;
                            if (hapi.ValidIDs.Intersect(Id.ToLower().Split('_')).Count() > 0) // ex: id=rbspicea_l0_aux
                            {
                                // HACK: May fail given more spacecraft options.
                                string[] valArr = val.Split('_');
                                if (valArr.Count() >= 0)
                                    SC = valArr[(int)IndexOf.SC];
                                if (valArr.Count() >= 1)
                                    Level = valArr[(int)IndexOf.Level];
                                if (valArr.Count() >= 2)
                                    RecordType = valArr[(int)IndexOf.RecordType];
                            }
                            else
                            {
                                ErrorCodes.Add(1406);
                            }
                            break;

                        case ("time.min"):
                            dt = cons.ConvertHapiYMDToDateTime(val);
                            if (dt != default(DateTime))
                                TimeMin = dt.ToUniversalTime();
                            else
                                ErrorCodes.Add(1402);
                            break;

                        case ("time.max"):
                            dt = cons.ConvertHapiYMDToDateTime(val);
                            if (dt != default(DateTime) && TimeMin < dt)
                                TimeMax = dt.ToUniversalTime();
                            else if (dt == default(DateTime))
                                ErrorCodes.Add(1403);
                            else if (TimeMin >= dt)
                                ErrorCodes.Add(1404);
                            break;

                        case ("parameters"):
                            Parameters = val.Split(new char[] { ',' }).ToList();
                            break;

                        case ("include"):
                            if (val == "header")
                                IncludeHeader = true;
                            else
                                ErrorCodes.Add(1410);
                            break;

                        case ("format"):
                            if (hapi.Formats.Contains(val))
                                Format = val;
                            else
                                ErrorCodes.Add(1409);
                            break;

                        default:
                            ErrorCodes.Add(1400);
                            break;
                    }
                }

                if (ErrorCodes.Count() > 0)
                    return false;
                else
                    return true;
            }

            private bool RequestParametersValid(string requestType, Dictionary<string, string> dict)
            {
                List<string> paramsRequired;
                List<string> paramsOptional;

                switch (requestType)
                {
                    case ("data"):
                        paramsRequired = new List<string> { "id", "time.min", "time.max" };
                        paramsOptional = new List<string> { "parameters", "include", "format" };
                        break;

                    case ("info"):
                        paramsRequired = new List<string> { "id" };
                        paramsOptional = new List<string> { "parameters" };
                        break;
                    default:
                        ErrorCodes.Add(1400);
                        return false;
                }

                List<string> requestParams = new List<string>();
                List<int> errors = new List<int>();

                IEnumerable<string> allParamsForRequest;

                allParamsForRequest = paramsRequired.Concat(paramsOptional);
                foreach (KeyValuePair<string, string> pair in dict)
                {
                    if (!(allParamsForRequest.Contains(pair.Key)))
                    {
                        errors.Add(1401);
                        return false;
                    }
                    requestParams.Add(pair.Key);
                }

                if (!(paramsRequired.Intersect(requestParams).Count() == paramsRequired.Count()))
                    errors.Add(1401);

                return true;
            }

            public override string ToString()
            {
                string pars = String.Empty;
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

        internal class ErrorResponse : ResponseContent
        {
            public string[] OutputFormats { get; }

            public ErrorResponse(HapiConfiguration hapi)
            {
                HapiVersion = hapi.Version;
                Status = new Status();
                OutputFormats = hapi.Capabilities;
            }

            public override string GetResponse()
            {
                StringBuilder sb = new StringBuilder();
                return sb.Append(String.Format(
                    "{{\n" +
                    "\t\"{0}\" : \"{1}\",\n" +
                    "\t\"{2}\" : {{ \"code\" : {3}, \"message\" : \"{4}\" }},\n" +
                    "}}\n",
                    "Hapi",
                    HapiVersion,
                    "status",
                    Status.Code,
                    Status.Message
                )).ToString();
            }
        }

        internal class CapabilitiesResponse : ResponseContent
        {
            public string[] OutputFormats { get; }

            public CapabilitiesResponse(HapiConfiguration hapi)
            {
                HapiVersion = hapi.Version;
                Status = new Status();
                OutputFormats = hapi.Capabilities;
            }

            public override string GetResponse()
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

        internal class DataResponse : ResponseContent
        {
            public string StartDate = String.Empty;
            public string StopDate = String.Empty;
            public string Format = String.Empty;
            public List<string> Parameters = null;
            public IEnumerable<Dictionary<string, string>> Data = null;

            public DataResponse(HapiConfiguration hapi)
            {
                Hapi = hapi;
                if (Hapi.Properties == null)
                    throw new MissingFieldException(nameof(Hapi.Properties));

                if (Hapi.Properties.TimeMin == null)
                    throw new MissingFieldException(nameof(Hapi.Properties.TimeMin));

                if (Hapi.Properties.TimeMax == null)
                    throw new MissingFieldException(nameof(Hapi.Properties.TimeMax));

                if (Hapi.Product == null)
                    throw new MissingFieldException(nameof(Hapi.Product));

                HapiVersion = Hapi.Version;
                StartDate = Hapi.Properties.TimeMin.ToString();
                StopDate = Hapi.Properties.TimeMax.ToString();
                Parameters = Hapi.Properties.Parameters;
                Format = Hapi.Properties.Format;
                Status = new Status();
                Data = Hapi.Product.Records;
            }

            public override string GetResponse()
            {
                string resp = String.Empty;

                switch (Format.ToLower())
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

        internal class InfoResponse : ResponseContent
        {
            private string Format { get; set; }
            private DateTime StartDate { get; set; }
            private DateTime StopDate { get; set; }
            private List<string> Parameters { get; set; }

            public InfoResponse(HapiConfiguration hapi)
            {
                Hapi = hapi;
                HapiVersion = hapi.Version;
                Status = new Status(); // HACK: don't use a literal value for code
                Format = Hapi.Properties.Format == null ? "csv" : Hapi.Properties.Format;
                StartDate = Hapi.Properties.TimeMin;
                StopDate = Hapi.Properties.TimeMax;
                Parameters = Hapi.Properties.Parameters;
            }

            public override string GetResponse()
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

        internal class CatalogResponse : ResponseContent
        {
            private List<Tuple<string, string>> Catalog { get; set; }

            public CatalogResponse(HapiConfiguration hapi)
            {
                Hapi = hapi;
                HapiVersion = Hapi.Version;
                Status = new Status();
                Catalog = new List<Tuple<string, string>>()
                {
                    Tuple.Create("RBSPICEA_L0_AUX", "RBSPA Level 0 Auxiliary Data"),
                };
            }

            private KeyValuePair<string, string> GetKeyValPair(string id, string title)
            {
                return new KeyValuePair<string, string>(id, title);
            }

            public override string GetResponse()
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

                foreach (var tuple in Catalog)
                {
                    sb.Append(
                        String.Format(
                            "\t\t{{ \"id\" : \"{0}\", \"title\" : \"{1}\" }},\n",
                            tuple.Item1,
                            tuple.Item2
                        )
                    );
                }

                sb.Append("\t]\n}");

                return sb.ToString();
            }
        }

        #endregion Helper Classes

        #region Internal Classes

        internal abstract class ResponseContent
        {
            public HapiConfiguration Hapi;
            public string HapiVersion { get; set; }
            public Status Status { get; set; }

            public static ResponseContent Create(HapiConfiguration hapi, string type)
            {
                if (type == "data")
                    return new DataResponse(hapi);

                if (type == "info")
                    return new InfoResponse(hapi);

                if (type == "catalog")
                    return new CatalogResponse(hapi);

                if (type == "capabilities")
                    return new CapabilitiesResponse(hapi);

                return null;
            }

            public abstract string GetResponse();

            public void SetStatusCode(int statusCode)
            {
                if (Status == null)
                    Status = new Status();

                Status.Code = statusCode;
            }
        }

        internal class Status
        {
            public Dictionary<int, string> ErrorCodes = new Dictionary<int, string>
            {
                { 1200, "OK" },
                { 1201, "OK - no data for time range" },
                { 1400, "Bad request - user input error" },
                { 1401, "Bad request - unknown API parameter name" },
                { 1402, "Bad request - error in start time" },
                { 1403, "Bad request - error in stop time" },
                { 1404, "Bad request - start time equal to or after stop time" },
                { 1405, "Bad request - time outside valid range" },
                { 1406, "Bad request - unknown dataset id" },
                { 1407, "Bad request - unknown dataset parameter" },
                { 1408, "Bad request - too much time or data requested" },
                { 1409, "Bad request - unsupported output format" },
                { 1410, "Bad request - unsupported include value" },
                { 1500, "Internal server error" },
                { 1501, "Internal server error - upstream request error" },
            };
            private int _code;
            public int Code { get { return _code; } set { Message = ErrorCodes[value]; _code = value; } }
            public string Message { get; private set; }
        }

        #endregion Internal Classes
    }
}