using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WebApi_v1.Hapi;
using WebApi_v1.Hapi.Utilities;
using static WebApi_v1.Hapi.Utilities.CSVHelperUtilities.Mappings;

namespace WebApi_v1.DataProducts.SpaceCraft.RBSpiceA.Products.AuxiliaryProduct
{
    public class AuxiliaryRecords
    {
        private HapiConfiguration HapiConfig { get; set; }
        private List<Dictionary<string,string>> Data { get; set; }

        public AuxiliaryRecords(HapiConfiguration hapi)
        {
            if (hapi != null)
                HapiConfig = hapi;
        }


        public IEnumerable<Dictionary<string,string>> GetRecords(IEnumerable<string> paths)
        {
            Data = new List<Dictionary<string, string>>();
            foreach (string path in paths)
            {
                if (File.Exists(path))
                {
                    using (TextReader textReader = new StreamReader(File.OpenRead(path)))
                    {
                        // TODO: Try and make this less dependant on Auxiliary type.
                        CsvReader csv = new CsvReader(textReader);

                        csv.Configuration.RegisterClassMap<Aux_Map>();
                        csv.Configuration.MissingFieldFound = null;
                        csv.Read();
                        csv.ReadHeader();
                        //TODO: Figure out best way to display header.
                        //aux.Header = csv.Context.HeaderRecord;

                        // If parameters exist read csv row by row and extract specific fields
                        // else convert all rows to records and save to this.Records
                        // HACK: Figure out a way to save a record with only the requested fields
                        Converters cons = new Converters();
                        if (HapiConfig.Properties.Parameters.Count > 0)
                        {
                            string[] headers = csv.Context.HeaderRecord;

                            for (int i = 0; i < headers.Length; i++)
                                headers[i] = headers[i].ToLower();
                            while (csv.Read())
                            {
                                // HACK: This is pretty hacky stuff.

                                // If csvrecord time is less than time.min or csvrecord
                                // time is greater than time.max then continue while loop.
                                // Inclusive min and Exclusive max
                                bool ltmin = cons.ConvertUTCtoDate(csv["UTC"]) < HapiConfig.Properties.TimeMin;
                                bool gtmax = cons.ConvertUTCtoDate(csv["UTC"]) >= HapiConfig.Properties.TimeMax;
                                if (ltmin || gtmax)
                                    continue;

                                AuxRecord aux = new AuxRecord();
                                foreach (string param in HapiConfig.Properties.Parameters)
                                {
                                    string parameterName = param;
                                    int indexOfParameterName = Array.IndexOf(headers, parameterName.ToLower());
                                    // TODO: This fails on receiving a bad parameter name in query.
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
                            {
                                headers[i] = headers[i].ToLower();
                                HapiConfig.Properties.Parameters.Add(headers[i]);
                            }

                            while (csv.Read())
                            {
                                // HACK: This is pretty hacky stuff.
                                //Auxiliary aux = new Auxiliary();

                                // If csvrecord time is less than time.min or
                                // csvrecord time is greater than time.max then break while loop.
                                bool ltmin = cons.ConvertUTCtoDate(csv["UTC"]) < HapiConfig.Properties.TimeMin;
                                bool gtmax = cons.ConvertUTCtoDate(csv["UTC"]) > HapiConfig.Properties.TimeMax;
                                if (ltmin || gtmax)
                                    continue;

                                Auxiliary rec = csv.GetRecord<Auxiliary>();
                                AuxRecord auxrec = new AuxRecord();
                                string utcString;
                                string propName;
                                foreach (System.Reflection.PropertyInfo prop in rec.GetType().GetProperties())
                                {
                                    propName = prop.Name.ToLower();
                                    if (headers.Contains(propName))
                                    {
                                        if (propName == "utc")
                                        {
                                            utcString = cons.ConvertDatetoUTCDate((DateTime)prop.GetValue(rec));
                                            auxrec.Add(propName, utcString);
                                            continue;
                                        }
                                        auxrec.Add(prop.Name, prop.GetValue(rec).ToString());
                                    }
                                }
                                Data.Add(auxrec.Data);
                            }
                        }
                    };
                }
            }
            return Data;
        }




    }
}