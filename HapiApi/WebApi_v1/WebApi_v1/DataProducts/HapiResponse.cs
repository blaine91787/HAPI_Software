using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace WebApi_v1.DataProducts
{
    internal interface IResponse
    {
        string GetResponse();

        string GetResponse(string format);
    }

    public static partial class Hapi
    {
        private static readonly string _version = "2.0";
        public static string[] Capabilities { get; } = { "csv", "json", "binary" };

        public class Status
        {
            public int Code = -1;
            public string Message = null;

            public Status(int code, string message)
            {
                Code = code;
                Message = message;
            }
        }

        public class DataResponse : IResponse
        {
            public string HapiVersion = _version;
            public Status Status = null;
            public string StartDate = String.Empty;
            public string StopDate = String.Empty;
            public List<string> Parameters = null;
            public string Format = "json";
            public IEnumerable<Dictionary<string, string>> Data = null;

            public DataResponse()
            {
                IProperties props = Hapi.Properties;
                if (props == null)
                    throw new MissingFieldException(nameof(Hapi.Properties));

                if (props.TimeMin == null)
                    throw new MissingFieldException(nameof(props.TimeMin));

                if (props.TimeMax == null)
                    throw new MissingFieldException(nameof(props.TimeMax));

                if (Hapi.Product == null)
                    throw new MissingFieldException(nameof(Hapi.Product));

                StartDate = props.TimeMin.ToString();
                StopDate = props.TimeMax.ToString();
                Parameters = props.Parameters;
                Status = new Status(1200, "OK");
                Data = Hapi.Product.Records;
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
                bool multiLineParameters = false;
                StringBuilder sb = new StringBuilder();
                sb.Append(String.Format(
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
                ));

                if (Hapi.Properties.Parameters != null)
                {
                    foreach (string param in Hapi.Properties.Parameters)
                    {
                        if (multiLineParameters)
                        {
                            sb.Append(String.Format(
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
                            ));
                        }
                        else
                        {
                            sb.Append(String.Format(
                                "#\t\t{{ \"name\" : \"{0}\", \"type\" : \"{1}\", \"units\" : \"{2}\", \"fill\" : \"{3}\", \"length\" : {4} }},\n",
                                param.ToLower(),
                                "null",
                                "null",
                                "null",
                                "null"
                            ));
                        }
                    }
                }

                sb.Append("#\t],\n#}\n");

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
                    //KeyValuePair<string, string>[] dataArr = rec.ToArray(); ;

                    foreach (KeyValuePair<string, string> pair in rec)
                    {
                        sb.Append(pair.Value);
                        if (pair.Key != last)
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
                        Hapi.Properties.TimeMin,
                        Hapi.Properties.TimeMax
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
                                    param,
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
                                    param,
                                    "null",
                                    "null",
                                    "null",
                                    "null"
                                ));
                            }
                        }
                    }

                    sb.Append("\t],\n"); // trailing ']' for parameters

                    sb.Append(String.Format(
                        "\t\"format\" : \"{0}\",\n",
                        "json"
                    ));

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

        public class CapabilitiesResponse
        {
            public string HapiVersion { get; }
            public Status Status { get; }
            public string[] OutputFormats { get; }

            public CapabilitiesResponse()
            {
                HapiVersion = _version;
                Status = new Status(1200, "OK");
                OutputFormats = Hapi.Capabilities;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                return sb.Append(String.Format(
                    "{{\n" +
                    "\t\"{0}\" : \"{1}\",\n" +
                    "\t\"{2}\" : {{ \"code\" : {3}, \"message\" : \"{4}\" }},\n" +
                    "\t\"{5}\" : [ \"{6}\", \"{7}\", \"{8}\" ]\n" +
                    "}}\n",
                    "Hapi",
                    HapiVersion,
                    "status",
                    Status.Code,
                    Status.Message,
                    "outputFormats",
                    OutputFormats[0],
                    OutputFormats[1],
                    OutputFormats[2]
                )).ToString();
            }
        }

        public class InfoResponse : IResponse
        {
            private string HapiVersion { get; set; }
            private Status Status { get; set; }
            private string Format { get; set; }
            private DateTime StartDate { get; set; }
            private DateTime StopDate { get; set; }
            private List<string> Parameters { get; set; }

            public InfoResponse()
            {
                HapiVersion = _version;
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
            private string HapiVersion { get; set; }
            private Status Status { get; set; }
            private List<KeyValuePair<string, string>> Catalog { get; set; }

            public CatalogResponse()
            {
                HapiVersion = _version;
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
    }
}