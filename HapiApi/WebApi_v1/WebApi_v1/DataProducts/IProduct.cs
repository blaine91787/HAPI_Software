using System.Collections.Generic;

namespace WebApi_v1.DataProducts
{
    public interface IProduct
    {
        string[] Header { get; set; }
        List<DataRecord> Records { get; set; }
        List<Dictionary<string, string>> ParameterSpecificRecords { get; set; }

        void GetProduct();

        void GetProductWithTimeRange();
    }
}