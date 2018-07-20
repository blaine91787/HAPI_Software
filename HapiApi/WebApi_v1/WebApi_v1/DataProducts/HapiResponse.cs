using System;
using System.Collections.Generic;
using System.Net.Http;

namespace WebApi_v1.DataProducts
{
    public class HapiResponse
    {
        public string Hapi = "2.0";
        public Status status = new Status();
        public string startDate = String.Empty;
        public string stopDate = String.Empty;
        public List<string> parameters = null;
        public string format = "json";
        public IEnumerable<Dictionary<string, string>> data = null;

        public HapiResponse(IEnumerable<Dictionary<string, string>> records)
        {
            data = records;
        }

        public class Status
        {
            public int code = 1200;
            public string Message = "OK";
        }
    }
}