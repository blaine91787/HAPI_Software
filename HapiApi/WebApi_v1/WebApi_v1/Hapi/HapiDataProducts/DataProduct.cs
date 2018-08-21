using System;
using System.Collections.Generic;
using System.IO;
using WebApi_v1.HAPI.Configuration;
using WebApi_v1.HAPI.DataProducts.SpaceCraft.RBSPA;
using WebApi_v1.HAPI.Properties;

namespace WebApi_v1.HAPI.DataProducts
{
    public abstract class HapiDataProduct
    {
        public Hapi Hapi { get; set; }
        public List<string> Paths { get; set; }
        public List<FileInfo> Files { get; set; }

        public string[] Header { get; set; }
        public IEnumerable<Dictionary<string, string>> Records { get; set; }

        public static HapiDataProduct Create(string scName, Hapi hapi)
        {
            if (hapi.Properties == null)
                throw new ArgumentNullException("HapiProperties is null.");

            if (hapi.Configuration == null)
                throw new ArgumentNullException("HapiConfiguration is null");

            HapiDataProduct product = null;
            switch (scName)
            {
                case ("rbspicea"):
                    product = new RBSpiceAProduct();
                    product.Configure(hapi);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Not a valid spacecraft.");
            }

            return product;
        }
        public abstract void Configure(Hapi hapi);
        public abstract bool GetProduct();
        public abstract bool VerifyTimeRange();
        public abstract void GetPaths();
    }
}