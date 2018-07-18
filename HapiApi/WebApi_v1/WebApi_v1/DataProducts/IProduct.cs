using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi_v1.DataProducts
{
    public interface IProduct
    {
        string[] Header { get; set; }
        List<DataRecord> Records { get; set; }
        List<Dictionary<string,string>> ParameterSpecificRecords { get; set; }
        void GetProduct();
        void GetProductWithTimeRange();
    }
}
