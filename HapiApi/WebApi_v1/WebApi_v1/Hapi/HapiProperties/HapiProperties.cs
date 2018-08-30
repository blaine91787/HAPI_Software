using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApi_v1.HAPI.Configuration;
using WebApi_v1.HAPI.Utilities;
using WebApi_v1.HAPI.Response;
using WebApi_v1.HAPI.Catalog;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace WebApi_v1.HAPI.Properties
{
    public class HapiProperties
    {
        public HttpRequestMessage Request { get; private set; }
        public string RequestType { get; private set; }
        public string RequestQuery { get; private set; }
        public string ID { get; private set; }
        public string SC { get; private set; }
        public string Instrument { get; private set; }
        public string Product { get; private set; }
        public string Format { get; private set; }
        public bool IncludeHeader { get; private set; }
        public TimeRange TimeRange { get; private set; }
        public List<string> Parameters { get; private set; }
        public List<Status.HapiStatusCode> ErrorCodes { get; private set; }

        #region Private Methods

        private void Initialize()
        {
            RequestType = String.Empty;
            RequestQuery = String.Empty;
            ID = String.Empty;
            SC = String.Empty;
            Instrument = String.Empty;
            Product = String.Empty;
            Format = "csv";
            IncludeHeader = false;
            TimeRange = new TimeRange();
            Parameters = new List<string>();
            ErrorCodes = new List<Status.HapiStatusCode>();
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

                case ("catalog"): // No parameters required/optional
                    return true;

                case ("capabilities"):
                    return true; // No parameters required/optional

                default:
                    ErrorCodes.Add(Status.HapiStatusCode.UserInputError);
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
                    ErrorCodes.Add(Status.HapiStatusCode.UnknownAPIParameterName);
                    return false;
                }
                requestParams.Add(pair.Key);
            }

            if (!(paramsRequired.Intersect(requestParams).Count() == paramsRequired.Count()))
            {
                ErrorCodes.Add(Status.HapiStatusCode.UnknownAPIParameterName);
                return false;
            }

            return true;
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hapi"></param>
        /// <returns></returns>
        public bool Assign(HttpRequestMessage request, HapiConfiguration hapiConfig, HapiCatalog catalog)
        {
            
            Initialize();

            Request = request;
            RequestType = request.RequestUri.LocalPath.Split('/').Last().ToLower();

            if (RequestType == String.Empty)
                throw new InvalidOperationException("HapiConfiguration must be configured.");

            RequestQuery = request.RequestUri.Query;
            Dictionary<string, string> dict = CreateQueryDictionary(RequestQuery);

            if (!(RequestParametersValid(RequestType, dict)))
                ErrorCodes.Add(Status.HapiStatusCode.UnknownAPIParameterName);

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
                        ID = val;
                        
                        if (catalog.IsValidProduct(ID)) // ex: id=rbspicea_l0_aux
                        {
                            // HACK: May fail given more spacecraft options.
                            int sc = 0;
                            int instr = 1;
                            int prod = 2;
                            string[] idArray = val.Split('_'); // id=rbspicea_l0_aux
                            SC = idArray[sc];
                            Instrument = idArray[instr];
                            Product = idArray[prod];
                        }
                        else
                        {
                            ErrorCodes.Add(Status.HapiStatusCode.UnknownDatasetID);
                            return false;
                        }
                        break;

                    case ("time.min"):
                        dt = cons.ConvertHapiYMDToDateTime(val);
                        if (dt != default(DateTime))
                        {
                            TimeRange.UserMin = dt.ToUniversalTime();
                        }
                        else
                        {
                            ErrorCodes.Add(Status.HapiStatusCode.ErrorInStartTime);
                            return false;
                        }
                        break;

                    case ("time.max"):
                        dt = cons.ConvertHapiYMDToDateTime(val);
                        if (dt != default(DateTime))
                            TimeRange.UserMax = dt.ToUniversalTime();
                        else if (dt == default(DateTime))
                        {
                            ErrorCodes.Add(Status.HapiStatusCode.ErrorInStopTime);
                            return false;
                        }
                        else if (TimeRange.UserMax >= dt)
                        {
                            ErrorCodes.Add(Status.HapiStatusCode.StartTimeEqualToOrAfterStopTime);
                            return false;
                        }

                        if (TimeRange.UserMin == TimeRange.UserMax)
                        {
                            ErrorCodes.Add(Status.HapiStatusCode.StartTimeEqualToOrAfterStopTime);
                            return false;
                        }
                        break;

                    case ("parameters"):
                        Parameters = val.Split(new char[] { ',' }).ToList();
                        break;

                    case ("include"):
                        if (val == "header")
                        {
                            IncludeHeader = true;
                        }
                        else
                        {
                            ErrorCodes.Add(Status.HapiStatusCode.UnsupportedIncludeValue);
                            return false;
                        }
                        break;

                    case ("format"):
                        if (hapiConfig.Formats.Contains(val))
                        {
                            Format = val;
                        }
                        else
                        {
                            ErrorCodes.Add(Status.HapiStatusCode.UnsupportedOuputFormat);
                            return false;
                        }
                        break;

                    default:
                        ErrorCodes.Add(Status.HapiStatusCode.UserInputError);
                        return false;
                }
            }
            return true;
        }

        private Dictionary<string, string> CreateQueryDictionary(string hapiQuery)
        {
            if (hapiQuery == null)
                throw new ArgumentNullException(nameof(hapiQuery));

            char[] delimiters = { '?', '&', '=' };
            Dictionary<string, string> dict = new Dictionary<string, string>();;

            string[] arr = hapiQuery.ToLower().TrimStart(delimiters).Split(delimiters);

            if (arr.Length >= 2 && arr.Length % 2 == 0)
            {
                for (int i = 0; i < arr.Length; i += 2)
                    dict.Add(arr[i], arr[i + 1]);
            }
            else if (RequestType == "data") // Query empty or 
            {
                ErrorCodes.Add(Status.HapiStatusCode.UserInputError);
                return dict;
            }

            return dict;
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
                ID,
                TimeRange.UserMin == default(DateTime) ? "No start time provided." : TimeRange.UserMin.ToString(),
                TimeRange.UserMax == default(DateTime) ? "No end time provided." : TimeRange.UserMax.ToString(),
                pars,
                IncludeHeader
            );
        }

        #endregion Public Methods
    }
}