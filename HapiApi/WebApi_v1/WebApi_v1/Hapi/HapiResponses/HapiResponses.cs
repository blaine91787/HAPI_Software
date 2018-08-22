using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using WebApi_v1.HAPI.Configuration;
using WebApi_v1.HAPI.Catalog;

namespace WebApi_v1.HAPI.Response
{
    public abstract class Content
    {
        public Hapi Hapi { get; set; }
        public string HapiVersion { get; set; }
        public Status Status { get; set; }

        public static Content Create(Hapi hapi, string type)
        {
            if (type == "data")
                return new Data(hapi);

            if (type == "info")
                return new Info(hapi);

            if (type == "catalog")
                return new Catalog(hapi);

            if (type == "capabilities")
                return new Capabilities(hapi);

            if (type == "error")
                return new Error(hapi);

            return null;
        }

        public abstract string GetResponse();

        public void SetStatusCode(Status.HapiStatusCode statusCode)
        {
            if (Status == null)
                Status = new Status();

            Status.Code = (int)statusCode;
        }
    }

    public class Status
    {
        public enum HapiStatusCode
        {
            OK = 1200,
            OKNoDataForTimeRange = 1201,
            UserInputError = 1400,
            UnknownAPIParameterName = 1401,
            ErrorInStartTime = 1402,
            ErrorInStopTime = 1403,
            StartTimeEqualToOrAfterStopTime = 1404,
            TimeOutsideValidRange = 1405,
            UnknownDatasetID = 1406,
            UnknownDatasetParameter = 1407,
            TooMuchTimeOrDataRequested = 1408,
            UnsupportedOuputFormat = 1409,
            UnsupportedIncludeValue = 1410,
            InternalServerError = 1500,
            UpstreamRequestError = 1501
        }

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

    internal class Capabilities : Content
    {
        private string[] OutputFormats { get; }

        public Capabilities(Hapi hapi)
        {
            HapiVersion = hapi.Configuration.Version;
            Status = new Status();
            OutputFormats = hapi.Configuration.Capabilities;
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

    internal class Data : Content
    {
        private string StartDate = String.Empty;
        private string StopDate = String.Empty;
        private string Format = String.Empty;
        private List<string> Parameters = null;
        private IEnumerable<Dictionary<string, string>> DataRecords = null;

        public Data(Hapi hapi)
        {
            Hapi = hapi ?? throw new ArgumentNullException("HapiConfiguration not configured.");

            if (Hapi.Properties == null)
                throw new MissingFieldException(nameof(Hapi.Properties));

            if (Hapi.Properties.TimeMin == null)
                throw new MissingFieldException(nameof(Hapi.Properties.TimeMin));

            if (Hapi.Properties.TimeMax == null)
                throw new MissingFieldException(nameof(Hapi.Properties.TimeMax));

            if (Hapi.DataProduct == null)
                throw new MissingFieldException(nameof(Hapi.DataProduct));

            HapiVersion = Hapi.Configuration.Version;
            StartDate = Hapi.Properties.TimeMin.ToString();
            StopDate = Hapi.Properties.TimeMax.ToString();
            Parameters = Hapi.Properties.Parameters;
            Format = Hapi.Properties.Format;
            Status = new Status();
            DataRecords = Hapi.DataProduct.Records;
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

        private string ToCSV()
        {
            bool header = Hapi.Properties.IncludeHeader;
            StringBuilder sb = new StringBuilder();

            if (header)
            {
                sb.Append(GetInfoHeader());
            }

            string last;
            if (Hapi.DataProduct.Records.Count() > 0)
            {
                last = Hapi.DataProduct.Records.ToList().First().ToList().Last().Key;
            }
            else
            {
                return ("No records were found. If this is an error, make sure query is valid.");
            }

            foreach (Dictionary<string, string> rec in Hapi.DataProduct.Records)
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

        private string ToJson()
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
                last = Hapi.DataProduct.Records.ToList().First().ToList().Last().Key;
            }
            catch
            {
                throw new InvalidOperationException("\"HapiResponse.ToJson()>string last\" is empty. Possibly missing time.min or time.max or invalid request.");
            }

            foreach (Dictionary<string, string> rec in Hapi.DataProduct.Records)
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

    internal class Info : Content
    {
        private string Format { get; set; }
        private DateTime StartDate { get; set; }
        private DateTime StopDate { get; set; }
        private List<Field> Parameters { get; set; }

        public Info(Hapi hapi)
        {
            Hapi = hapi ?? throw new ArgumentNullException("Configuration not configured.");

            HapiVersion = Hapi.Configuration.Version;
            Status = new Status(); // HACK: don't use a literal value for code
            Format = Hapi.Properties.Format;
            StartDate = Hapi.Properties.TimeMin;
            StopDate = Hapi.Properties.TimeMax;
            Parameters = Hapi.Catalog.GetProduct(Hapi.Properties.ID).GetFields();

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
                foreach (Field param in Parameters)
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
                            param.Name,
                            param.Type,
                            param.Units,
                            param.Fill,
                            param.Length
                        ));
                    }
                    else
                    {
                        sb.Append(String.Format(
                            "\t\t{{ \"name\" : \"{0}\", \"type\" : \"{1}\", \"units\" : \"{2}\", \"fill\" : \"{3}\", \"length\" : {4} }},\n",
                            param.Name,
                            param.Type,
                            param.Units,
                            param.Fill,
                            param.Length                            
                        ));
                    }
                }
            }

            sb.Append("\t],\n}\n");

            return sb.ToString();
        }
    }

    internal class Catalog : Content
    {
        public Catalog(Hapi hapi)
        {
            Hapi = hapi ?? throw new ArgumentNullException("HapiConfiguration not configured.");

            HapiVersion = Hapi.Configuration.Version;
            Status = new Status();
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

            foreach (Product prod in Hapi.Catalog.GetProducts())
            {
                sb.Append(
                    String.Format(
                        "\t\t{{ \"id\" : \"{0}\", \"title\" : \"{1}\" }},\n",
                        prod.HapiId,
                        prod.Title
                    )
                );
            }

            sb.Append("\t]\n}");

            return sb.ToString();
        }
    }

    internal class Error : Content
    {
        private string[] OutputFormats { get; }

        public Error(Hapi hapi)
        {
            Hapi = hapi ?? throw new ArgumentNullException("HapiConfiguration not configured.");

            HapiVersion = Hapi.Configuration.Version;
            Status = new Status();
            OutputFormats = Hapi.Configuration.Capabilities;
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
}