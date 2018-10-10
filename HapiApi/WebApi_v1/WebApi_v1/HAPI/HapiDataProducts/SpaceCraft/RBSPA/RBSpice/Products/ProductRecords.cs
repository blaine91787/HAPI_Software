using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WebApi_v1.HAPI.HapiDataProducts.SpaceCraft.RBSPA.RBSpice.Products.ESRHELT;
using WebApi_v1.HAPI.HapiDataProducts.SpaceCraft.RBSPA.RBSpice.Products.ESRLEHT;
using WebApi_v1.HAPI.HapiDataProducts.SpaceCraft.RBSPA.RBSpice.Products.TOFxEH;
using WebApi_v1.HAPI.HapiDataProducts.SpaceCraft.RBSPA.RBSpice.Products.TOFxEHe;
using WebApi_v1.HAPI.HapiDataProducts.SpaceCraft.RBSPA.RBSpice.Products.TOFxEIon;
using WebApi_v1.HAPI.HapiDataProducts.SpaceCraft.RBSPA.RBSpice.Products.TOFxEO;
using WebApi_v1.HAPI.HapiDataProducts.SpaceCraft.RBSPA.RBSpice.Products.TOFxPHHHELT;
using WebApi_v1.HAPI.HapiDataProducts.SpaceCraft.RBSPA.RBSpice.Products.TOFxPHHLEHT;
using WebApi_v1.HAPI.HapiDataProducts.SpaceCraft.RBSPA.RBSpice.Products.TOFxPHOHELT;
using WebApi_v1.HAPI.HapiDataProducts.SpaceCraft.RBSPA.RBSpice.Products.TOFxPHOLEHT;
using WebApi_v1.HAPI.Utilities;

namespace WebApi_v1.HAPI.HapiDataProducts.SpaceCraft.RBSPA.RBSpice.Products
{
    public abstract class ProductRecords
    {
        public Hapi Hapi { get; set; }
        public List<Dictionary<string,string>> Data { get; set; }
        //public abstract Hapi Hapi { get; set; }
        //public abstract List<Dictionary<string,string>> Data { get; set; }
        public static ProductRecords Create(Hapi hapi, string type)
        {
            ProductRecords pr = default(ProductRecords);

            switch (type)
            {
                case ("esrhelt"):
                    pr = new EsrheltRecords(hapi);
                    break;
                case ("esrleht"):
                    pr = new EsrlehtRecords(hapi);
                    break;
                case ("tofxeh"):
                    pr = new TofxehRecords(hapi);
                    break;
                case ("tofxehe"):
                    pr = new TofxeheRecords(hapi);
                    break;
                case ("tofxeion"):
                    pr = new TofxeionRecords(hapi);
                    break;
                case ("tofxeo"):
                    pr = new TofxeoRecords(hapi);
                    break;
                case ("tofxphhhelt"):
                    pr = new TofxphhheltRecords(hapi);
                    break;
                case ("tofxphhleht"):
                    pr = new TofxphhlehtRecords(hapi);
                    break;
                case ("tofxphohelt"):
                    pr = new TofxphoheltRecords(hapi);
                    break;
                case ("tofxpholeht"):
                    pr = new TofxpholehtRecords(hapi);
                    break;
                default:
                    throw new Exception("ProductRecords factory was unable to determine product type.");
            }
            return pr;
        }

        public IEnumerable<Dictionary<string, string>> GetRecords(IEnumerable<FileInfo> files)
        {
            Data = new List<Dictionary<string, string>>();
            if (files.Count() == 0)
                return Data;

            string fiExt = files.FirstOrDefault().Extension; // TODO: Change this from using FirstOrDefault to using the xml catalog extension type
            if (fiExt == ".cdf")
            {
                foreach (FileInfo fi in files)
                {
                    CDFReader cdf = new CDFReader(fi.FullName);
                    string[] parameters = Hapi.Properties.Parameters.ToArray();
                    DateTime minTime = Hapi.Properties.TimeRange.UserMin;
                    DateTime maxTime = Hapi.Properties.TimeRange.UserMax;
                    Data.AddRange(cdf.GetCDFHapiRecords(minTime, maxTime, parameters));
                    cdf.Close();
                }
            }
            return Data;
        }
    }
}