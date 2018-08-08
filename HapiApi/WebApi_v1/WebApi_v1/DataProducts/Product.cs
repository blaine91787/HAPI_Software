using System.Collections.Generic;

namespace WebApi_v1.DataProducts
{
    public abstract class Product : IProduct
    {
        public Dictionary<string, string> SearchParameters { get; set; }
        public IEnumerable<Dictionary<string, string>> Records { get; set; }
        public List<Dictionary<string, string>> ParameterSpecificRecords { get; set; }
        public string[] Header { get; set; }

        public abstract bool GetProduct();
        public abstract bool VerifyTimeRange();
    }
}