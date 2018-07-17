using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using WebApi_v1.DataProducts.Utilities;
using static WebApi_v1.DataProducts.CSVHelperUtilities.TypeConverters;

namespace WebApi_v1.DataProducts
{
    public class Auxiliary : DataRecord
    {
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

    public sealed class AuxiliaryMap : ClassMap<Auxiliary>
    {
        public AuxiliaryMap()
        {
            AutoMap();
            Map(m => m.UTC).Name( "UTC" ).TypeConverter<ConvertUTCtoDateTime>();
        }
    }
}