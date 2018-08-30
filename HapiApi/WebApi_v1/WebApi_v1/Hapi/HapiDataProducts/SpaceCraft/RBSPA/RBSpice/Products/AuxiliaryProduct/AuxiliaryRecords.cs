using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using WebApi_v1.HAPI.Utilities;
using WebApi_v1.HAPI.Configuration;
using WebApi_v1.HAPI.Utilities.CSVHelperUtilities;

namespace WebApi_v1.HAPI.DataProducts.SpaceCraft.RBSPA.RBSpice.Auxiliary
{
    public class AuxiliaryRecords
    {
        private Hapi Hapi { get; set; }
        private List<Dictionary<string,string>> Data { get; set; }

        public AuxiliaryRecords(Hapi hapi)
        {
            if (hapi != null)
                Hapi = hapi;
        }


        public IEnumerable<Dictionary<string,string>> GetRecords(IEnumerable<string> paths)
        {
            Data = new List<Dictionary<string, string>>();
            foreach (string path in paths)
            {
                if (File.Exists(path))
                {
                    FileInfo fileToDecompress = new FileInfo(path); ;

                    using (FileStream originalFileStream = fileToDecompress.OpenRead())
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    using (TextReader decompressedFileReader = new StreamReader(decompressionStream))
                    {
                        // TODO: Try and make this less dependant on Auxiliary type.
                        CsvReader csv = new CsvReader(decompressedFileReader);

                        csv.Configuration.RegisterClassMap<Mappings.Aux_Map>();
                        csv.Configuration.MissingFieldFound = null;
                        csv.Read();
                        csv.ReadHeader();
                        //TODO: Figure out best way to display header.
                        //aux.Header = csv.Context.HeaderRecord;

                        // If parameters exist read csv row by row and extract specific fields
                        // else convert all rows to records and save to this.Records
                        // HACK: Figure out a way to save a record with only the requested fields
                        Converters cons = new Converters();
                        if (Hapi.Properties.Parameters.Count > 0)
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
                                bool ltmin = cons.ConvertUTCtoDate(csv["UTC"]) < Hapi.Properties.TimeRange.UserMin;
                                bool gtmax = cons.ConvertUTCtoDate(csv["UTC"]) >= Hapi.Properties.TimeRange.UserMax;
                                if (ltmin || gtmax)
                                    continue;

                                AuxRecord aux = new AuxRecord();
                                foreach (string param in Hapi.Properties.Parameters)
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
                                Hapi.Properties.Parameters.Add(headers[i]);
                            }

                            while (csv.Read())
                            {
                                // HACK: This is pretty hacky stuff.
                                //Auxiliary aux = new Auxiliary();

                                // If csvrecord time is less than time.min or
                                // csvrecord time is greater than time.max then break while loop.
                                bool ltmin = cons.ConvertUTCtoDate(csv["UTC"]) < Hapi.Properties.TimeRange.UserMin;
                                bool gtmax = cons.ConvertUTCtoDate(csv["UTC"]) > Hapi.Properties.TimeRange.UserMax;
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