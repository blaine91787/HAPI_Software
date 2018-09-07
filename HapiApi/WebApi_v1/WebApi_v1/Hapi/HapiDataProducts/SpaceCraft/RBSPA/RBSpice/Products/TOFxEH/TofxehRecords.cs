using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WebApi_v1.HAPI.Utilities;

namespace WebApi_v1.HAPI.DataProducts.SpaceCraft.RBSPA.RBSpice.TOFxEH
{
    public class TofxehRecords
    {
        private Hapi Hapi { get; set; }
        private List<Dictionary<string, string>> Data { get; set; }

        public TofxehRecords(Hapi hapi)
        {
            if (hapi != null)
                Hapi = hapi;
        }

        public IEnumerable<Dictionary<string, string>> GetRecords(IEnumerable<FileInfo> files)
        {
            Data = new List<Dictionary<string, string>>();
            if (files.Count() == 0)
                return null;

            string fiExt = files.FirstOrDefault().Extension;
            if (fiExt == ".cdf")
            {
                foreach (FileInfo fi in files)
                {
                    CDFReader cdf = new CDFReader(fi.FullName);
                    string[] parameters = Hapi.Properties.Parameters.ToArray();
                    DateTime minTime = Hapi.Properties.TimeRange.UserMin;
                    DateTime maxTime = Hapi.Properties.TimeRange.UserMax;
                    Data.AddRange(cdf.GetCDFHapiRecords(minTime, maxTime, parameters));
                }
            }
            return Data;
        }
    }
}