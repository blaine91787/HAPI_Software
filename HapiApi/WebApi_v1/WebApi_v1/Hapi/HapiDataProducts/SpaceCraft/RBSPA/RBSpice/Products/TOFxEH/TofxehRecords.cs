using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WebApi_v1.HAPI.Utilities;

namespace WebApi_v1.HAPI.DataProducts.SpaceCraft.RBSPA.RBSpice.TOFxEH
{
    public class Tofxeh
    {
        public string Epoch { get; set; }
        public string UTC { get; set; }
        public string DDOY { get; set; }
        public string ET { get; set; }
        public string MidET { get; set; }
        public string StopET { get; set; }
        public string Duration { get; set; }
        public string OrbitNumber { get; set; }
        public string Spin { get; set; }
        public string FPDU { get; set; }
        public string FPDUWeight { get; set; }
        public string FPDUPerpPressure { get; set; }
        public string FPDUParaPressure { get; set; }
        public string FPDUDensity { get; set; }
        public string FPDUIntegralFlux { get; set; }
        public string FPDUOmniFlux { get; set; }
        public string FPDUMinimFlux { get; set; }
        public string FPDUMaximFlux { get; set; }
        public string FPDUError { get; set; }
        public string FPDUEnergy { get; set; }
        public string FPDUEnergyRange { get; set; }
        public string FPDUQuality { get; set; }
        public string Position { get; set; }
        public string PositionSM { get; set; }
        public string PositionGSM { get; set; }
        public string PositionQuality { get; set; }
        public string L { get; set; }
        public string MLT { get; set; }
        public string LEq { get; set; }
        public string LStar { get; set; }
        public string I { get; set; }
        public string PAMidpoint { get; set; }
        public string PARange { get; set; }
        public string Channel { get; set; }
        public string Bin { get; set; }
        public string Axis { get; set; }
        public string MinMaxRange { get; set; }
    }
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
                return Data;

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
                    cdf.Close();
                }
            }
            return Data;
        }
    }
}