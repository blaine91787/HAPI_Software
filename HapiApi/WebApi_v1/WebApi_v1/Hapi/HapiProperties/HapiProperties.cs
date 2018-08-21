using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApi_v1.Hapi.Utilities;
using WebApi_v1.Hapi.Response;

namespace WebApi_v1.Hapi
{
    public class Properties
    {

        public string RequestType { get; private set; }
        public string Id { get; private set; }
        public string SC { get; private set; }
        public string Level { get; private set; }
        public string RecordType { get; private set; }
        public string Format { get; private set; }
        public bool IncludeHeader { get; private set; }
        public DateTime TimeMin { get; private set; }
        public DateTime TimeMax { get; private set; }
        public List<string> Parameters { get; private set; }
        public List<Status.HapiStatusCode> ErrorCodes { get; private set; }

        #region Private Methods

        private void Initialize()
        {
            RequestType = String.Empty;
            Id = String.Empty;
            SC = String.Empty;
            Level = String.Empty;
            RecordType = String.Empty;
            Format = "csv";
            IncludeHeader = false;
            TimeMin = default(DateTime);
            TimeMax = default(DateTime);
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

        public bool Assign(Configuration hapi)
        {
            if (hapi.QueryDict == null || hapi.RequestType == String.Empty)
                throw new InvalidOperationException("HapiConfiguration must be configured.");
            
            Initialize();

            Dictionary<string, string> dict = hapi.QueryDict;

            if (!(RequestParametersValid(hapi.RequestType, dict)))
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
                        Id = val;
                        if (hapi.ValidIDs.Intersect(Id.ToLower().Split('_')).Count() > 0) // ex: id=rbspicea_l0_aux
                        {
                            // HACK: May fail given more spacecraft options.
                            int id = 0;
                            int level = 1;
                            int recordType = 2;
                            string[] idArray = val.Split('_'); // id=rbspicea_l0_aux
                            if (idArray.Count() >= 0)
                                SC = idArray[id];
                            if (idArray.Count() >= 1)
                                Level = idArray[level];
                            if (idArray.Count() >= 2)
                                RecordType = idArray[recordType];
                        }
                        else
                        {
                            ErrorCodes.Add(Status.HapiStatusCode.UnknownDatasetID);
                        }
                        break;

                    case ("time.min"):
                        dt = cons.ConvertHapiYMDToDateTime(val);
                        if (dt != default(DateTime))
                            TimeMin = dt.ToUniversalTime();
                        else
                            ErrorCodes.Add(Status.HapiStatusCode.ErrorInStartTime);
                        break;

                    case ("time.max"):
                        dt = cons.ConvertHapiYMDToDateTime(val);
                        if (dt != default(DateTime) && TimeMin < dt)
                            TimeMax = dt.ToUniversalTime();
                        else if (dt == default(DateTime))
                            ErrorCodes.Add(Status.HapiStatusCode.ErrorInStopTime);
                        else if (TimeMin >= dt)
                            ErrorCodes.Add(Status.HapiStatusCode.StartTimeEqualToOrAfterStopTime);
                        break;

                    case ("parameters"):
                        Parameters = val.Split(new char[] { ',' }).ToList();
                        break;

                    case ("include"):
                        if (val == "header")
                            IncludeHeader = true;
                        else
                            ErrorCodes.Add(Status.HapiStatusCode.UnsupportedIncludeValue);
                        break;

                    case ("format"):
                        if (hapi.Formats.Contains(val))
                            Format = val;
                        else
                            ErrorCodes.Add(Status.HapiStatusCode.UnsupportedOuputFormat);
                        break;

                    default:
                        ErrorCodes.Add(Status.HapiStatusCode.UserInputError);
                        break;
                }
            }

            return (ErrorCodes.Count() > 0) ? false : true;
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

        #endregion Public Methods
    }
}