using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using WebApi_v1.DataProducts.Utilities;
using static WebApi_v1.DataProducts.CSVHelperUtilities.TypeConverters;

namespace WebApi_v1.DataProducts
{
    public class Auxiliary : DataRecord
    {
        // TODO: Figure out what to do with this UTC and the abstract UTC
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

        public Auxiliary()
        {
            // HACK: Using defaults to determine which properties are set may be bad practice???

            foreach (System.Reflection.PropertyInfo prop in this.GetType().GetProperties())
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                Converters.ConvertPropertyToDefault(prop, this);
            }
        }

        public void AddField(string parameterName, string parameterValue)
        {
            foreach (System.Reflection.PropertyInfo prop in this.GetType().GetProperties())
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                if (string.Equals(prop.Name, parameterName, StringComparison.OrdinalIgnoreCase))
                {
                    Converters.ConvertParameterToProperty(parameterValue, prop, this);
                    return;
                }
                //prop =  parameter;
                //str += prop.GetValue(this, null).ToString() + ", ";
            }
            throw new ArgumentOutOfRangeException(parameterName);
            //switch (parameter.ToLower())
            //{
            //    case ("sclock_full"):
            //        SCLOCK_Full = parameter;
            //        return;
            //    case ("et"):
            //        ET = parameter;
            //        return;
            //    case ("orbitnumber"):
            //        OrbitNumber = parameter;
            //        return;
            //    case ("seconds"):
            //        Seconds = parameter;
            //        return;
            //Subseconds
            //Spin
            //SpinTime
            //SpinPeriod
            //SpinPhase
            //MagPhase
            //Mag0Second1
            //Mag0Subsecond1
            //Mag0Second2
            //Mag0Subsecond2
            //IntegrationSectors
            //IntegrationMultiplier1
            //IntegrationMultiplier2
            //IntegrationSpins
            //MagPhaseValid
            //MagDataValid
            //SpinDataValid
            //ElectronPixel
            //IonEnergyPixel
            //IonSpeciesPixel
            //Subsector1
            //Subsector2
            //Subsector3
            //SpinDuration
        }
    }

    public class AuxiliaryByParameters : DataRecord
    {
        public Dictionary<string, string> Record { get; set; }

        public AuxiliaryByParameters()
        {
            Record = new Dictionary<string, string>();
        }
    }

    /// <summary>
    /// A mapping used by CSVHelper library to convert the CSV version of time to DateTime
    /// so it can be compared when searching for time.min and time.max parameters.
    /// </summary>
    public sealed class AuxiliaryMap : ClassMap<Auxiliary>
    {
        public AuxiliaryMap()
        {
            AutoMap();
            Map(m => m.UTC).Name("UTC").TypeConverter<ConvertUTCtoDateTime>();
        }
    }
}