using CsvHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApi_v1.HAPI.DataProducts;

namespace WebApi_v1.HAPI.DataProducts.SpaceCraft.RBSPA.RBSpice.Auxiliary
{
    public class AuxRecord
    {
        //public string[] Header { get; set; }
        public Dictionary<string, string> Data { get; set; }

        public AuxRecord()
        {
            Data = new Dictionary<string, string>();
        }

        public void Add(string key, string val)
        {
            Data.Add(key, val);
        }

        public string RecordToString()
        {
            StringBuilder sb = new StringBuilder();

            KeyValuePair<string, string>[] dataArr = Data.ToArray();

            for (int i = 0; i < dataArr.Length; i++)
            {
                sb.Append("\"" + dataArr[i].Value + "\"");
                if (i != dataArr.Length - 1)
                    sb.Append(",\n");
            }

            sb.Append("]\n");
            return sb.ToString();
        }
    }


    public class Auxiliary
    {
        public DateTime UTC { get; set; }
        public string SCLOCK_Full { get; set; }
        public string ET { get; set; }
        public string OrbitNumber { get; set; }
        public Int64 Seconds { get; set; }
        public Int32 Subseconds { get; set; }
        public Int32 Spin { get; set; }
        public Int64 SpinTime { get; set; }
        public Int32 SpinPeriod { get; set; }
        public Int32 SpinPhase { get; set; }
        public Int32 MagPhase { get; set; }
        public Byte Mag0Second1 { get; set; }
        public Byte Mag0Subsecond1 { get; set; }
        public Byte Mag0Second2 { get; set; }
        public Byte Mag0Subsecond2 { get; set; }
        public Byte IntegrationSectors { get; set; }
        public Byte IntegrationMultiplier1 { get; set; }
        public Byte IntegrationMultiplier2 { get; set; }
        public Byte IntegrationSpins { get; set; }
        public Boolean MagPhaseValid { get; set; }
        public Boolean MagDataValid { get; set; }
        public Boolean SpinDataValid { get; set; }
        public Boolean ElectronPixel { get; set; }
        public Boolean IonEnergyPixel { get; set; }
        public Boolean IonSpeciesPixel { get; set; }
        public Int16 Subsector1 { get; set; }
        public Int16 Subsector2 { get; set; }
        public Int16 Subsector3 { get; set; }
        public Int32 SpinDuration { get; set; }
    }
}