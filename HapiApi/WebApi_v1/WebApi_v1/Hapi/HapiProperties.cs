using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApi_v1.HapiUtilities;

namespace WebApi_v1.Hapi
{
    public class HapiProperties
    {
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
    }
}