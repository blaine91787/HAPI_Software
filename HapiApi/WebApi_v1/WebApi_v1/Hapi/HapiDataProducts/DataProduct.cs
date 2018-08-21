using System;
using System.Collections.Generic;
using System.IO;
using WebApi_v1.Hapi.DataProducts.SpaceCraft.RBSPA;

namespace WebApi_v1.Hapi.DataProducts
{
    public abstract class DataProduct
    {
        public Configuration HapiConfig { get; set; }
        public List<string> Paths { get; set; }
        public List<FileInfo> Files { get; set; }

        public string[] Header { get; set; }
        public IEnumerable<Dictionary<string, string>> Records { get; set; }

        public static DataProduct Create(Configuration hapi)
        {
            if (hapi == null)
                throw new ArgumentNullException("Configuration hapi");

            DataProduct temp = null;
            switch(hapi.Properties.SC)
            {
                case ("rbspicea"):
                    temp = new RBSpiceAProduct();
                    temp.Configure(hapi);
                    temp.GetProduct();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Not a valid spacecraft.");
            }

            return temp;
        }
        public abstract void Configure(Configuration hapi);
        public abstract bool GetProduct();
        public abstract bool VerifyTimeRange();
        public abstract void GetPaths();
    }
}