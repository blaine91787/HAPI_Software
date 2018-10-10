using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApi_v1.HAPI.Catalog;
using WebApi_v1.HAPI.Utilities;
using Newtonsoft.Json;

namespace WebApi_v1.HAPI.Response
{
    public class ResponseBody
    {
        public String HAPI { get; set; }
        public Status Status { get; set; }
        public string Format { get; set; }
        public string StartDate { get; set; }
        public string StopDate { get; set; }
        public List<Object> Parameters { get; set; }
        public List<Dictionary<string, string>> Data { get; set; }
    }
    public abstract class Content
    {
        public Hapi Hapi { get; set; }
        public string HapiVersion { get; set; }
        public Status Status { get; set; }

        public static Content Create(Hapi hapi, string type)
        {
            if (type == "data")
                try { return new Data(hapi); }
                catch (Exception e) { if (false) { throw e; } else { } }

            if (type == "info")
                try { return new Info(hapi); }
                catch (Exception e) { if (false) { throw e; } else { } }

            if (type == "catalog")
                try { return new Catalog(hapi); }
                catch (Exception e) { if (false) { throw e; } else { } }

            if (type == "capabilities")
                try { return new Capabilities(hapi); }
                catch (Exception e) { if (false) { throw e; } else { } }

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

        private Dictionary<int, string> ErrorCodes = new Dictionary<int, string>
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

    //internal class Data : Content
    //{
    //    private string StartDate = String.Empty;
    //    private string StopDate = String.Empty;
    //    private string Format = String.Empty;
    //    private List<string> Parameters = null;
    //    private IEnumerable<Dictionary<string, string>> DataRecords = null;

    //    public Data(Hapi hapi)
    //    {
    //        Hapi = hapi ?? throw new ArgumentNullException("HapiConfiguration not configured.");

    //        if (Hapi.Properties == null)
    //            throw new MissingFieldException(nameof(Hapi.Properties));

    //        if (Hapi.Properties.TimeRange.UserMin == null)
    //            throw new MissingFieldException(nameof(Hapi.Properties.TimeRange.UserMin));

    //        if (Hapi.Properties.TimeRange.UserMax == null)
    //            throw new MissingFieldException(nameof(Hapi.Properties.TimeRange.UserMax));

    //        if (Hapi.DataProduct == null)
    //            throw new MissingFieldException(nameof(Hapi.DataProduct));

    //        HapiVersion = Hapi.Configuration.Version;
    //        StartDate = Hapi.Properties.TimeRange.UserMin.ToString();
    //        StopDate = Hapi.Properties.TimeRange.UserMax.ToString();
    //        Parameters = Hapi.Properties.Parameters;
    //        Format = Hapi.Properties.Format;
    //        Status = new Status();
    //        DataRecords = Hapi.DataProduct.Records;
    //    }

    //    public override string GetResponse()
    //    {
    //        string resp = String.Empty;

    //        switch (Format.ToLower())
    //        {
    //            case ("csv"):
    //                resp = ToCSV();
    //                return resp;

    //            case ("json"):
    //                resp = ToJson();
    //                return resp;

    //            default:
    //                return resp;
    //        }
    //    }

    //    private string GetInfoHeader()
    //    {
    //        // TODO: Make GetInfoHeader work for both csv and json versions. Possibly make it it's own helper class.
    //        // This would allow it to be used for the other responses, not just DataResponse.
    //        // Also, figure out what to do about multiline parameters.
    //        bool multiLineParameters = false;
    //        StringBuilder sb = new StringBuilder();
    //        sb.AppendFormat(
    //            "#{{\n" +
    //            "#\t\"HAPI\" : \"{0}\",\n" +
    //            "#\t\"status\" : {{ \"code\" : {1}, \"message\" : \"{2}\" }},\n" +
    //            "#\t\"startDate\" : \"{3}\",\n" +
    //            "#\t\"stopDate\" : \"{4}\",\n" +
    //            "#\t\"parameters\" : [\n",
    //            HapiVersion,
    //            Status.Code,
    //            Status.Message,
    //            Hapi.Properties.TimeRange.UserMin,
    //            Hapi.Properties.TimeRange.UserMax
    //        );

    //        Dictionary<string, Field> catParams = Hapi.Catalog.GetProduct(Hapi.Properties.ID).Fields;
    //        if (Hapi.Properties.Parameters != null && catParams != null)
    //        {
    //            foreach (string param in Hapi.Properties.Parameters)
    //            {
    //                if (catParams.ContainsKey(param))
    //                {
    //                    if (multiLineParameters)
    //                    {
    //                        sb.AppendFormat(
    //                            "#\t\t{{\n " +
    //                            "#\t\t   \"name\" : \"{0}\",\n" +
    //                            "#\t\t   \"type\" : \"{1}\",\n" +
    //                            "#\t\t   \"units\" : \"{2}\",\n" +
    //                            "#\t\t   \"fill\" : \"{3}\",\n" +
    //                            "#\t\t   \"length\" : {4},\n" +
    //                            "#\t\t}},\n",
    //                            catParams[param].Name,
    //                            catParams[param].Type,
    //                            catParams[param].Units,
    //                            catParams[param].Fill,
    //                            catParams[param].Length
    //                        );
    //                    }
    //                    else
    //                    {
    //                        sb.AppendFormat(
    //                            "#\t\t{{ \"name\" : \"{0}\", \"type\" : \"{1}\", \"units\" : \"{2}\", \"fill\" : \"{3}\", \"length\" : {4} }},\n",
    //                            catParams[param].Name,
    //                            catParams[param].Type,
    //                            catParams[param].Units,
    //                            catParams[param].Fill,
    //                            catParams[param].Length
    //                        );
    //                    }
    //                }
    //            }
    //        }

    //        sb.AppendFormat(
    //            "#\t],\n#\t\"format\" : \"{0}\",\n#}}\n",
    //            Format
    //        );

    //        return sb.ToString();
    //    }

    //    private string ToCSV()
    //    {
    //        bool header = Hapi.Properties.IncludeHeader;
    //        StringBuilder sb = new StringBuilder();

    //        if (header)
    //        {
    //            sb.Append(GetInfoHeader());
    //        }

    //        string last;
    //        if (Hapi.DataProduct.Records.Count() > 0)
    //        {
    //            // Grab the first record and the last field's name in that record to compare
    //            // later whether a new line should be created.
    //            last = Hapi.DataProduct.Records.ToList().First().ToList().Last().Key; //TODO: fails when fpduenergy is requested. "Sequence contains no elements"
    //        }
    //        else
    //        {
    //            return ("No records were found. If this is an error, make sure query is valid.");
    //        }

    //        // For each record, iterate through the fields and append the value separated by commas
    //        foreach (Dictionary<string, string> rec in Hapi.DataProduct.Records)
    //        {
    //            foreach (KeyValuePair<string, string> pair in rec)
    //            {
    //                if (pair.Value.Contains(','))
    //                {
    //                    sb.AppendFormat("[{0}],", pair.Value.TrimEnd(','));
    //                }
    //                else
    //                {
    //                    sb.AppendFormat("{0},", pair.Value);
    //                }
    //            }
    //            string temp = sb.ToString().TrimEnd(',');
    //            temp.Replace(",,", ",");
    //            sb.Clear();
    //            sb.Append(temp + "\n");
    //        }
    //        return sb.ToString();
    //    }

    //    private string ToJson()
    //    {
    //        bool multiLineParameters = false;
    //        bool header = true;// Hapi.Properties.IncludeHeader;
    //        StringBuilder sb = new StringBuilder();

    //        if (header)
    //        {
    //            sb.AppendFormat(
    //                "{{\n" +
    //                "\t\"HAPI\" : \"{0}\",\n" +
    //                "\t\"status\" : {{ \"code\" : {1}, \"message\" : \"{2}\" }},\n" +
    //                "\t\"startDate\" : \"{3}\",\n" +
    //                "\t\"stopDate\" : \"{4}\",\n" +
    //                "\t\"parameters\" : [\n",
    //                HapiVersion,
    //                Status.Code,
    //                Status.Message,
    //                Hapi.Properties.TimeRange.UserMin,
    //                Hapi.Properties.TimeRange.UserMax
    //            );

    //            Dictionary<string, Field> catParams = Hapi.Catalog.GetProduct(Hapi.Properties.ID).Fields;
    //            if (Hapi.Properties.Parameters != null && catParams != null)
    //            {
    //                foreach (string param in Hapi.Properties.Parameters)
    //                {
    //                    if (catParams.ContainsKey(param))
    //                    {
    //                        if (multiLineParameters)
    //                        {
    //                            sb.AppendFormat(
    //                                "\t\t{{\n " +
    //                                "\t\t   \"name\" : \"{0}\",\n" +
    //                                "\t\t   \"type\" : \"{1}\",\n" +
    //                                "\t\t   \"units\" : \"{2}\",\n" +
    //                                "\t\t   \"fill\" : \"{3}\",\n" +
    //                                "\t\t   \"length\" : {4},\n" +
    //                                "\t\t}},\n",
    //                                catParams[param].Name,
    //                                catParams[param].Type,
    //                                catParams[param].Units,
    //                                catParams[param].Fill,
    //                                catParams[param].Length
    //                            );
    //                        }
    //                        else
    //                        {
    //                            sb.AppendFormat(
    //                                "\t\t{{ \"name\" : \"{0}\", \"type\" : \"{1}\", \"units\" : \"{2}\", \"fill\" : \"{3}\", \"length\" : {4} }},\n",
    //                                catParams[param].Name,
    //                                catParams[param].Type,
    //                                catParams[param].Units,
    //                                catParams[param].Fill,
    //                                catParams[param].Length
    //                            );
    //                        }
    //                    }
    //                }
    //            }

    //            string temp = sb.ToString().TrimEnd(',');
    //            sb.Remove(0, sb.Length);
    //            sb.Append(temp);

    //            sb.Append("\t],\n"); // trailing ']' for parameters

    //            sb.AppendFormat(
    //                "\t\"format\" : \"{0}\",\n",
    //                Format
    //            );

    //            sb.Append("\t\"data\" : [\n");
    //        }

    //        string last;
    //        try
    //        {
    //            last = Hapi.DataProduct.Records.ToList().First().ToList().Last().Key;
    //        }
    //        catch
    //        {
    //            throw new InvalidOperationException("\"HapiResponse.ToJson()>string last\" is empty. Possibly missing time.min or time.max or invalid request.");
    //        }

    //        foreach (Dictionary<string, string> rec in Hapi.DataProduct.Records)
    //        {
    //            if (header)
    //                sb.Append("\t\t[");

    //            //KeyValuePair<string, string>[] dataArr = rec.ToArray(); ;

    //            foreach (KeyValuePair<string, string> pair in rec)
    //            {
    //                sb.Append(pair.Value.TrimEnd(','));
    //                if (pair.Key != last)
    //                    sb.Append(",");
    //            }

    //            if (header)
    //                sb.Append("],\n");
    //            else
    //                sb.Append("\n");
    //        }

    //        sb.Append("\t]\n}");

    //        JObject json = JObject.Parse(sb.ToString());
    //        return json.ToString();
    //    }
    //}

    internal class Data : Content
    {
        private ResponseBody Response { get; set; }

        public Data(Hapi hapi)
        {
            Hapi = hapi ?? throw new ArgumentNullException("HapiConfiguration not configured.");

            if (Hapi.Properties == null)
                throw new MissingFieldException(nameof(Hapi.Properties));

            if (Hapi.Properties.TimeRange.UserMin == null)
                throw new MissingFieldException(nameof(Hapi.Properties.TimeRange.UserMin));

            if (Hapi.Properties.TimeRange.UserMax == null)
                throw new MissingFieldException(nameof(Hapi.Properties.TimeRange.UserMax));

            if (Hapi.DataProduct == null)
                throw new MissingFieldException(nameof(Hapi.DataProduct));
        }
        public override string GetResponse()
        {
            if (Hapi.Properties.Format == "csv")
                return ToCSV();
            else if (Hapi.Properties.Format == "json")
                return ToJSON();
            else
                return String.Empty;
        }
        private string GetHeader()
        {
            Response = new ResponseBody();

            Response.HAPI = Hapi.Configuration.Version;
            Response.Status = new Status();
            Response.Status.Code = Status.Code;
            Response.Format = Hapi.Properties.Format;
            Response.StartDate = Hapi.Properties.TimeRange.UserMin.ToString();
            Response.StopDate = Hapi.Properties.TimeRange.UserMax.ToString();

            Response.Parameters = new List<Object>();
            Dictionary<string, Field> catParams = Hapi.Catalog.GetProduct(Hapi.Properties.ID).Fields;
            if (Hapi.Properties.Parameters.Count != 0 && catParams != null)
            {
                foreach (string param in Hapi.Properties.Parameters)
                {
                    if (catParams.ContainsKey(param))
                    {
                        Response.Parameters.Add(catParams[param]);
                    }
                }
            }
            else
            {
                foreach (var kvpair in catParams)
                {
                    Response.Parameters.Add(kvpair.Value);
                }
            }

            return JsonConvert.SerializeObject(Response);
        }

        private string ToJSON()
        {
            Response = new ResponseBody();

            Response.HAPI = Hapi.Configuration.Version;
            Response.Status = new Status();
            Response.Status.Code = Status.Code;
            Response.Format = Hapi.Properties.Format;
            Response.StartDate = Hapi.Properties.TimeRange.UserMin.ToString();
            Response.StopDate = Hapi.Properties.TimeRange.UserMax.ToString();

            Response.Parameters = new List<Object>();
            Dictionary<string, Field> catParams = Hapi.Catalog.GetProduct(Hapi.Properties.ID).Fields;
            if (Hapi.Properties.Parameters.Count != 0 && catParams != null)
            {
                foreach (string param in Hapi.Properties.Parameters)
                {
                    if (catParams.ContainsKey(param))
                    {
                        Response.Parameters.Add(catParams[param]);
                    }
                }
            }
            else
            {
                foreach (var kvpair in catParams)
                {
                    Response.Parameters.Add(kvpair.Value);
                }
            }

            Response.Data = Hapi.DataProduct.Records.ToList();

            return JsonConvert.SerializeObject(Response);
        }

        private string ToCSV()
        {
            StringBuilder sb = new StringBuilder();

            if (Hapi.Properties.IncludeHeader)
                sb.Append(GetCSVHeader());

            // For each record, iterate through the fields and append the value separated by commas
            foreach (Dictionary<string, string> rec in Hapi.DataProduct.Records)
            {
                foreach (KeyValuePair<string, string> pair in rec)
                {

                    sb.AppendFormat("{0},", pair.Value.Replace("[", "").Replace("]", ""));
                }
                string temp = sb.ToString().TrimEnd(',');
                sb.Clear();
                sb.Append(temp + "\n");
            }
            return sb.ToString();
        }

        private string GetCSVHeader()
        {
            // TODO: Make GetInfoHeader work for both csv and json versions. Possibly make it it's own helper class.
            // This would allow it to be used for the other responses, not just DataResponse.
            // Also, figure out what to do about multiline parameters.
            bool multiLineParameters = true;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(
                "#{{\n" +
                "#\t\"HAPI\" : \"{0}\",\n" +
                "#\t\"status\" : {{ \"code\" : {1}, \"message\" : \"{2}\" }},\n" +
                "#\t\"startDate\" : \"{3}\",\n" +
                "#\t\"stopDate\" : \"{4}\",\n" +
                "#\t\"parameters\" : [\n",
                Hapi.Configuration.Version,
                Status.Code,
                Status.Message,
                Hapi.Properties.TimeRange.UserMin,
                Hapi.Properties.TimeRange.UserMax
            );

            Dictionary<string, Field> catParams = Hapi.Catalog.GetProduct(Hapi.Properties.ID).Fields;
            if (Hapi.Properties.Parameters.Count != 0 && catParams != null)
            {
                foreach (string param in Hapi.Properties.Parameters)
                {
                    if (catParams.ContainsKey(param))
                    {
                        if (multiLineParameters)
                        {
                            sb.AppendFormat(
                                "#\t\t{{\n" +
                                "#\t\t   \"name\" : \"{0}\",\n" +
                                "#\t\t   \"type\" : \"{1}\",\n" +
                                "#\t\t   \"units\" : \"{2}\",\n" +
                                "#\t\t   \"fill\" : \"{3}\",\n" +
                                "#\t\t   \"length\" : {4}\n" +
                                "#\t\t}},\n",
                                catParams[param].Name,
                                catParams[param].Type,
                                catParams[param].Units,
                                catParams[param].Fill,
                                catParams[param].Length
                            );
                        }
                        else
                        {
                            sb.AppendFormat(
                                "#\t\t{{ \"name\" : \"{0}\", \"type\" : \"{1}\", \"units\" : \"{2}\", \"fill\" : \"{3}\", \"length\" : {4} }},\n",
                                catParams[param].Name,
                                catParams[param].Type,
                                catParams[param].Units,
                                catParams[param].Fill,
                                catParams[param].Length
                            );
                        }
                    }
                }
            }
            else
            {
                foreach (var param in catParams)
                {
                    if (multiLineParameters)
                    {
                        sb.AppendFormat(
                            "#\t\t{{\n" +
                            "#\t\t   \"name\" : \"{0}\",\n" +
                            "#\t\t   \"type\" : \"{1}\",\n" +
                            "#\t\t   \"units\" : \"{2}\",\n" +
                            "#\t\t   \"fill\" : \"{3}\",\n" +
                            "#\t\t   \"length\" : {4}\n" +
                            "#\t\t}},\n",
                            catParams[param.Key].Name,
                            catParams[param.Key].Type,
                            catParams[param.Key].Units,
                            catParams[param.Key].Fill,
                            catParams[param.Key].Length
                        );
                    }
                    else
                    {
                        sb.AppendFormat(
                            "#\t\t{{ \"name\" : \"{0}\", \"type\" : \"{1}\", \"units\" : \"{2}\", \"fill\" : \"{3}\", \"length\" : {4} }},\n",
                            catParams[param.Key].Name,
                            catParams[param.Key].Type,
                            catParams[param.Key].Units,
                            catParams[param.Key].Fill,
                            catParams[param.Key].Length
                        );
                    }
                    //Response.Parameters.Add(kvpair.Value);
                }
            }
            string temp = sb.ToString().Trim(new char[] {',','\n'});
            sb.Clear();
            sb.Append(temp + "\n");
            sb.AppendFormat(
                "#\t],\n#\t\"format\" : \"{0}\"\n#}}\n",
                Hapi.Properties.Format
            );

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

            Product product = default(Product);

            try
            {
                product = Hapi.Catalog.GetProduct(Hapi.Properties.ID);// TODO: Exception here when the id is entered incorrectly (e.g. rbspicea_rbspice_tofxeh)
            }
            catch (Exception exc)
            {
                Hapi.Properties.ErrorCodes.Add(Status.HapiStatusCode.UnknownDatasetID);
                throw exc;
            }

            Parameters = product.GetFields();

            TimeRange tr = Hapi.Properties.TimeRange;
            if (Hapi.Properties.ID == "rbspa_rbspice_auxil")
                tr.GetAvailableTimeRange(Hapi.Catalog.GetProduct(Hapi.Properties.ID).Path, out _, out _);
            else
                tr.GetAvailableTimeRange(Hapi.Catalog.GetProduct(Hapi.Properties.ID), out _, out _);
            StartDate = Hapi.Properties.TimeRange.Min;
            StopDate = Hapi.Properties.TimeRange.Max;
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

        private ResponseBody Response { get; set; }

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
            Response = new ResponseBody();

            Response.HAPI = Hapi.Configuration.Version;
            Response.Status = new Status();
            Response.Status.Code = Status.Code;
            JsonSerializerSettings jsonWriter = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(Response, Formatting.None, jsonWriter);
            //return sb.Append(String.Format(
            //    "{{\n" +
            //    "\t\"{0}\" : \"{1}\",\n" +
            //    "\t\"{2}\" : {{ \"code\" : {3}, \"message\" : \"{4}\" }},\n" +
            //    "}}\n",
            //    "Hapi",
            //    HapiVersion,
            //    "status",
            //    Status.Code,
            //    Status.Message
            //)).ToString();
        }
    }
}