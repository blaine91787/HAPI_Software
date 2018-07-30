using CsvHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using WebApi_v1.DataProducts.Utilities;
using static WebApi_v1.DataProducts.Utilities.CSVHelperUtilities.Mappings;

namespace WebApi_v1.DataProducts.RBSpiceA
{
    public class RBSpiceAProduct : IProduct
    {
        private string _basepath;

        public string[] Header { get; set; }

        public IEnumerable<Dictionary<string, string>> Records { get; set; }
        public List<Dictionary<string, string>> ParameterSpecificRecords { get; set; }
        public List<FileInfo> Files { get; set; }
        public IProperties HapiProperties { get; set; }
        public List<string> Paths { get; set; }

        public void Initialize()
        {
            string thinkpadpath = @"C:\Users\FTECS Account\\Documents\GitHub\FTECS\HapiApi\WebApi_v1\WebApi_v1\SCRecords\RBSPA\";
            string gazellepath = @"C:\Users\blaine.harris\Documents\GitHub\FTECS\HapiApi\WebApi_v1\WebApi_v1\SCRecords\RBSPA\";

            if (Directory.Exists(thinkpadpath))
                _basepath = thinkpadpath;
            else if (Directory.Exists(gazellepath))
                _basepath = gazellepath;
            else
                throw new DirectoryNotFoundException("RBSPiceAProduct._basepath could not resolve to a valid path.");
        }

        public RBSpiceAProduct(IProperties properties)
        {
            Initialize();

            if (properties != null)
                HapiProperties = properties;
            else
                throw new ArgumentNullException(nameof(properties));

            // HACK: Jerry may have a library for this.
            GetPaths();
        }

        private void GetPaths()
        {
            Paths = new List<string>();
            DateTime mintime = HapiProperties.TimeMin;
            DateTime maxtime = HapiProperties.TimeMax;
            DateTime mindate = mintime.Date;
            DateTime maxdate = maxtime.Date;
            string basepath = "";

            while (mintime <= maxtime)
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

                mintime = mintime.AddDays(1.0);
                mindate = mintime.Date;
            }
        }

        public bool Configure()
        {
            return true;
        }

        public void GetProduct() // TODO: change name to getRecords? Might be confusing with csvhelper though.
        {
            Auxiliary aux = new Auxiliary(HapiProperties);
            Records = aux.GetRecords(Paths);
        }

        public void GetProductWithTimeRange()
        {
            string path = @"C:\Users\blaine.harris\Documents\BlainesProjects\WebApi_v1\WebApi_v1\obj\Debug\rbsp-a-rbspice_lev-0_Auxil_20121028_v1.1.1-00.csv";
            Type recordtype;
            if ("Auxiliary" == nameof(Auxiliary))
                recordtype = typeof(Auxiliary);
            Records = new List<Dictionary<string, string>>();
            if (File.Exists(path))
            {
                using (TextReader textReader = new StreamReader(File.OpenRead(path)))
                {
                    CsvReader csv = new CsvReader(textReader);
                    csv.Configuration.RegisterClassMap<Aux_Map>();
                    csv.Read();
                    csv.ReadHeader();
                    this.Header = csv.Context.HeaderRecord;
                    //this.Records = csv.GetRecords<T>().ToList<T>();
                    while (csv.Read())
                    {
                        if (Converters.ConvertUTCtoDate(csv["UTC"]) >= TimeRange.Start
                            && Converters.ConvertUTCtoDate(csv["UTC"]) <= TimeRange.End)
                        {
                            //if ("Auxiliary" == nameof(Auxiliary))
                            //Records.Add(csv.GetRecord<IRecord>());
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