using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using CsvHelper;
using CsvHelper.TypeConversion;
using System.Diagnostics;
using WebApi_v1.DataProducts.Utilities;
using WebApi_v1.DataProducts;

namespace WebApi_v1.DataProducts
{
    public abstract class Product : IProduct
    {
        public Dictionary<string, string> SearchParameters { get; set;} 
        public List<DataRecord> Records { get; set; }
        public List<Dictionary<string,string>> ParameterSpecificRecords { get; set; }
        public string[] Header { get; set; }
        public abstract void GetProduct();
        public abstract void GetProductWithTimeRange();
    }
}