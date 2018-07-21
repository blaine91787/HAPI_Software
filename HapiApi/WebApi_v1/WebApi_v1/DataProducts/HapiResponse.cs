using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace WebApi_v1.DataProducts
{
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

        public class DataResponse
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

            public string ToJson()
            {
                bool cool = false;
                bool header = Hapi.Properties.IncludeHeader;
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
                            if (cool)
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
                        "json or csv or binary"
                    ));

                    sb.Append("\t\"data\" : [\n");
                }

                string last = Hapi.Product.Records.ToList().First().ToList().Last().Key;

                foreach (Dictionary<string, string> rec in Hapi.Product.Records)
                {
                    if (header)
                        sb.Append("\t\t[");

                    //KeyValuePair<string, string>[] dataArr = rec.ToArray(); ;

                    foreach (KeyValuePair<string, string> pair in rec)
                    {
                        sb.Append("\"" + pair.Value + "\"");
                        if (pair.Key != last)
                            sb.Append(",");
                    }

                    if (header)
                        sb.Append("],\n");
                    else
                        sb.Append("\n");
                }

                if (Hapi.Properties.IncludeHeader)
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

        public class InfoResponse
        {
        }
    }
}