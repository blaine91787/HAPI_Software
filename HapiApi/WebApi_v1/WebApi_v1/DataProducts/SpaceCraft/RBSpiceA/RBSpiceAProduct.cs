using CsvHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WebApi_v1.DataProducts.Utilities;

namespace WebApi_v1.DataProducts.RBSpiceA
{
    public class RBSpiceAProduct : IProduct, IConfigurable
    {
        private readonly string _basepath = @"C:\Users\FTECS Account\Documents\GitHub\FTECS\HapiApi\WebApi_v1\WebApi_v1\obj\Debug\";

        //private string path = @"C:\Users\blaine.harris\Documents\BlainesProjects\WebApi_v1\WebApi_v1\obj\Debug\rbsp-a-rbspice_lev-0_Auxil_20121028_v1.1.1-00.csv";
        public string[] Header { get; set; }

        public List<DataRecord> Records { get; set; }
        public List<Dictionary<string, string>> ParameterSpecificRecords { get; set; }
        public List<FileInfo> Files { get; set; }
        public IProperties HapiProperties { get; set; }
        public List<string> Paths { get; set; }

        public void Initialize()
        {
            Records = new List<DataRecord>();
            ParameterSpecificRecords = new List<Dictionary<string, string>>();
        }

        public RBSpiceAProduct()
        {
            Initialize();

            if (Hapi.Properties != null)
                HapiProperties = Hapi.Properties;
            else
                throw new ArgumentNullException(nameof(Hapi.Properties));

            // HACK: Jerry may have a library for this.
            GetPaths();
        }

        public RBSpiceAProduct(IProperties hapiProperties)
        {
            Initialize();

            if (hapiProperties != null)
                HapiProperties = hapiProperties;
            else
                throw new ArgumentNullException(nameof(hapiProperties));

            // HACK: Jerry may have a library for this.
            GetPaths();
        }

        private void GetPaths()
        {
            Paths = new List<string>();
            DateTime mindate = HapiProperties.TimeMin.Date;
            DateTime maxdate = HapiProperties.TimeMax.Date;
            string basepath = String.Empty;

            while (mindate <= maxdate)
            {
                basepath = _basepath;

                switch (HapiProperties.Level)
                {
                    case ("l0"):
                        basepath += @"Level_0\";
                        break;

                    default:
                        break;
                }

                switch (HapiProperties.RecordType)
                {
                    case ("aux"):
                        basepath += @"Auxil\";
                        basepath += mindate.ToString("yyyy") + @"\";
                        // TODO: implement gzip
                        basepath += String.Format(
                            "rbsp-a-rbspice_lev-0_Auxil_{0}_v1.1.1-00.csv",
                            mindate.ToString("yyyyMMdd")
                        );
                        break;

                    default:
                        break;
                }

                Paths.Add(basepath);

                mindate = mindate.AddDays(1.0);
            }
        }

        public bool Configure()
        {
            return true;
        }

        public void GetProduct() // TODO: change name to getRecords? Might be confusing with csvhelper though.
        {
            foreach (string path in Paths)
            {
                if (File.Exists(path))
                {
                    using (FileStream fs = File.OpenRead(path))
                    using (TextReader textReader = new StreamReader(fs))
                    {
                        // TODO: Try and make this less dependant on Auxiliary type.
                        CsvReader csv = new CsvReader(textReader);
                        csv.Configuration.RegisterClassMap<AuxiliaryMap>();
                        csv.Read();
                        csv.ReadHeader();
                        this.Header = csv.Context.HeaderRecord;

                        // If parameters exist read csv row by row and extract specific fields
                        // else convert all rows to records and save to this.Records
                        // HACK: Figure out a way to save a record with only the requested fields
                        if (HapiProperties.Parameters != null)
                        {
                            string[] headers = Header;

                            for (int i = 0; i < headers.Length; i++)
                                headers[i] = headers[i].ToLower();

                            while (csv.Read())
                            {
                                // HACK: This is pretty hacky stuff.
                                //Auxiliary aux = new Auxiliary();
                                AuxiliaryByParameters aux = new AuxiliaryByParameters();
                                //Dictionary<string, string> dict = new Dictionary<string, string>();
                                foreach (string param in HapiProperties.Parameters)
                                {
                                    string parameterName = param;
                                    int indexOfParameterName = Array.IndexOf(headers, parameterName);
                                    string parameterValue = csv[indexOfParameterName];
                                    aux.Record.Add(parameterName, parameterValue); // HACK: maybe get actual values, not just strings.
                                }

                                Records.Add(aux);
                            }
                        }
                        else
                        {
                            Records.AddRange(csv.GetRecords<Auxiliary>().ToList<DataRecord>());
                        }
                    }
                }
            }
        }

        public void GetProductWithTimeRange()
        {
            string path = @"C:\Users\blaine.harris\Documents\BlainesProjects\WebApi_v1\WebApi_v1\obj\Debug\rbsp-a-rbspice_lev-0_Auxil_20121028_v1.1.1-00.csv";
            Type recordtype;
            if ("Auxiliary" == nameof(Auxiliary))
                recordtype = typeof(Auxiliary);
            Records = new List<DataRecord>();
            if (File.Exists(path))
            {
                using (FileStream fs = File.OpenRead(path))
                using (TextReader textReader = new StreamReader(fs))
                {
                    CsvReader csv = new CsvReader(textReader);
                    csv.Configuration.RegisterClassMap<AuxiliaryMap>();
                    csv.Read();
                    csv.ReadHeader();
                    this.Header = csv.Context.HeaderRecord;
                    //this.Records = csv.GetRecords<T>().ToList<T>();
                    while (csv.Read())
                    {
                        if (Converters.ConvertUTCtoDate(csv["UTC"]) >= TimeRange.Start
                            && Converters.ConvertUTCtoDate(csv["UTC"]) <= TimeRange.End)
                        {
                            if ("Auxiliary" == nameof(Auxiliary))
                                Records.Add(csv.GetRecord<Auxiliary>());
                        }
                    }
                };
            }
            else
            {
                Debug.WriteLine("FAAAAAAIIIIIIILLLLLL");
            }
        }
    }
}