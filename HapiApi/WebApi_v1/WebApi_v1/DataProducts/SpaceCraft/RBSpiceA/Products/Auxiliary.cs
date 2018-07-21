using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebApi_v1.DataProducts.Utilities;
using static WebApi_v1.DataProducts.Utilities.CSVHelperUtilities.Mappings;

namespace WebApi_v1.DataProducts
{
    public class Auxiliary : IRecord
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
        public IProperties HapiProperties { get; private set; }
        public List<Dictionary<string, string>> Data { get; set; }

        public Auxiliary()
        {
            // HACK: Using defaults to determine which properties are set may be bad practice???

            foreach (System.Reflection.PropertyInfo prop in this.GetType().GetProperties())
            {
                Type type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                Converters.ConvertPropertyToDefault(prop, this);
            }

            HapiProperties = Hapi.Properties;
        }

        public IEnumerable<Dictionary<string, string>> GetRecords(IEnumerable<string> paths)
        {
            Data = new List<Dictionary<string, string>>();
            foreach (string path in paths)
            {
                if (File.Exists(path))
                {
                    using (FileStream fs = File.OpenRead(path))
                    using (TextReader textReader = new StreamReader(fs))
                    {
                        // TODO: Try and make this less dependant on Auxiliary type.
                        CsvReader csv = new CsvReader(textReader);

                        csv.Configuration.RegisterClassMap<Aux_Map>();
                        csv.Read();
                        csv.ReadHeader();
                        //TODO: Figure out best way to display header.
                        //aux.Header = csv.Context.HeaderRecord;

                        // If parameters exist read csv row by row and extract specific fields
                        // else convert all rows to records and save to this.Records
                        // HACK: Figure out a way to save a record with only the requested fields
                        if (Hapi.Properties.Parameters != null)
                        {
                            string[] headers = csv.Context.HeaderRecord;

                            for (int i = 0; i < headers.Length; i++)
                                headers[i] = headers[i].ToLower();

                            while (csv.Read())
                            {
                                // HACK: This is pretty hacky stuff.

                                // If csvrecord time is less than time.min or csvrecord
                                // time is greater than time.max then continue while loop.
                                bool ltmin = Converters.ConvertUTCtoDate(csv["UTC"]) <= HapiProperties.TimeMin;
                                bool gtmax = Converters.ConvertUTCtoDate(csv["UTC"]) >= HapiProperties.TimeMax;
                                if (ltmin || gtmax)
                                    continue;

                                AuxRecord aux = new AuxRecord();
                                //Dictionary<string, string> dict = new Dictionary<string, string>();
                                foreach (string param in HapiProperties.Parameters)
                                {
                                    string parameterName = param;
                                    int indexOfParameterName = Array.IndexOf(headers, parameterName);
                                    string parameterValue = csv[indexOfParameterName];
                                    aux.Add(parameterName, parameterValue); // HACK: maybe get actual values, not just strings.
                                }

                                Data.Add(aux.Data);
                            }
                        }
                        else
                        {
                            string[] headers = csv.Context.HeaderRecord;

                            for (int i = 0; i < headers.Length; i++)
                                headers[i] = headers[i].ToLower();

                            while (csv.Read())
                            {
                                // HACK: This is pretty hacky stuff.
                                //Auxiliary aux = new Auxiliary();

                                // If csvrecord time is less than time.min or
                                // csvrecord time is greater than time.max then break while loop.
                                bool ltmin = Converters.ConvertUTCtoDate(csv["UTC"]) <= HapiProperties.TimeMin;
                                bool gtmax = Converters.ConvertUTCtoDate(csv["UTC"]) >= HapiProperties.TimeMax;
                                if (ltmin || gtmax)
                                    continue;

                                //Dictionary<string, string> dict = new Dictionary<string, string>();
                                //IEnumerable csvrec = csv.GetRecord();
                                Auxiliary rec = csv.GetRecord<Auxiliary>();
                                AuxRecord auxrec = new AuxRecord();

                                foreach (System.Reflection.PropertyInfo prop in rec.GetType().GetProperties())
                                {
                                    //type type = nullable.getunderlyingtype(prop.propertytype) ?? prop.propertytype;
                                    //converters.convertpropertytodefault(prop, this);
                                    if (headers.Contains(prop.Name.ToLower()))
                                        auxrec.Add(prop.Name, prop.GetValue(rec).ToString());
                                }

                                Data.Add(auxrec.Data);
                                //foreach (string param in Hapi.Properties.Parameters)
                                //{
                                //    string parameterName = param;
                                //    int indexOfParameterName = Array.IndexOf(headers, parameterName);
                                //    string parameterValue = csv[indexOfParameterName];
                                //    aux.GetRecordUsingParameterInfo(parameterName, parameterValue); // HACK: maybe get actual values, not just strings.
                                //}

                                //Records.Add(aux);
                            }
                            //csv.GetRecords<Auxiliary>().ToList<AuxRecord>();
                        }
                    }
                }
            }
            return Data;
        }

        public new string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (System.Reflection.PropertyInfo prop in this.GetType().GetProperties())
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                sb.Append(prop.GetValue(this, null) + ", ");
            }

            return sb.ToString();
        }

        public class AuxRecord : IRecord
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

            public override string ToString()
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
    }
}