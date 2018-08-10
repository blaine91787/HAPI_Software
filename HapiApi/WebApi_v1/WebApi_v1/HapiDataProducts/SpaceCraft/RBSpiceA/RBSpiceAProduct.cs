using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebApi_v1.DataProducts.SpaceCraft.RBSpiceA.Products.AuxiliaryProduct;
using WebApi_v1.Hapi;
using WebApi_v1.HapiUtilities;

namespace WebApi_v1.HapiDataProducts.SpaceCraft.RBSpiceA.Products
{
    public class RBSpiceAProduct : Product
    {
        private string _basepath;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hapi"></param>
        public RBSpiceAProduct()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        public override void Configure(HapiConfiguration hapi)
        {
            if (hapi != null)
                HapiConfig = hapi;
            else
                throw new ArgumentNullException(nameof(hapi));


            string unicornpukepath = @"C:\Users\unicornpuke\\Documents\GitHub\FTECS\HapiApi\WebApi_v1\WebApi_v1\SCRecords\RBSPA\";
            string thinkpadpath = @"C:\Users\FTECS Account\\Documents\GitHub\FTECS\HapiApi\WebApi_v1\WebApi_v1\SCRecords\RBSPA\";
            string gazellepath = @"C:\Users\blaine.harris\Documents\Github\FTECS\HapiApi\WebApi_v1\WebApi_v1\SCRecords\RBSPA\";

            if (Directory.Exists(thinkpadpath))
                _basepath = thinkpadpath;
            else if (Directory.Exists(gazellepath))
                _basepath = gazellepath;
            else if (Directory.Exists(unicornpukepath))
                _basepath = unicornpukepath;
            else
                throw new DirectoryNotFoundException("RBSPiceAProduct._basepath could not resolve to a valid path.");

            GetPaths();
        }
        /// <summary>
        /// 
        /// </summary>
        public override void GetPaths()
        {
            Paths = new List<string>();
            DateTime mintime = HapiConfig.Properties.TimeMin;
            DateTime maxtime = HapiConfig.Properties.TimeMax;
            DateTime mindate = mintime.Date;
            DateTime maxdate = maxtime.Date;
            string basepath = String.Empty;

            while (mintime <= maxtime)
            {
                basepath = _basepath;

                switch (HapiConfig.Properties.Level)
                {
                    case ("l0"):
                        basepath += @"Level_0\";
                        break;

                    default:
                        break;
                }

                switch (HapiConfig.Properties.RecordType)
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
        /// <summary>
        /// 
        /// TODO: change name to getRecords? Might be confusing with csvhelper though.
        /// </summary>
        /// <returns></returns>
        public override bool GetProduct()
        {
            AuxiliaryRecords aux = new AuxiliaryRecords(HapiConfig);
            Records = aux.GetRecords(Paths);
            return Records.Count() != 0 ? true : false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool VerifyParameters()
        {

            return true;
        }
        /// <summary>
        /// 
        /// TODO: change name to getRecords? Might be confusing with csvhelper though.
        /// </summary>
        /// <returns></returns>
        public override bool VerifyTimeRange()
        {
            TimeRange tr = new TimeRange
            {
                UserMin = HapiConfig.Properties.TimeMin,
                UserMax = HapiConfig.Properties.TimeMax
            };

            string level = HapiConfig.Properties.Level.TrimStart('l');
            string recType = "";
            if (HapiConfig.Properties.RecordType != String.Empty)
            {
                switch (HapiConfig.Properties.RecordType)
                {
                    case ("aux"):
                        recType += @"Auxil\";
                        break;
                    default:
                        recType += HapiConfig.Properties.RecordType;
                        break;
                }
            }
            string path = String.Format(@"{0}Level_{1}\", _basepath, level, recType);
            var recTypePath = Directory.EnumerateDirectories(path).First();

            // We now have the path to the level and record type the user requested.

            // Now get the minimum possible time possible.
            var firstYearOfRecTypePath = Directory.EnumerateDirectories(recTypePath).First();
            var firstRecFileOfRecTypePath = Directory.EnumerateFiles(firstYearOfRecTypePath).First();
            path = firstRecFileOfRecTypePath;
            if (File.Exists(path))
            {
                using (TextReader textReader = new StreamReader(File.OpenRead(path)))
                {
                    CsvReader csv = new CsvReader(textReader);
                    csv.Read();
                    csv.ReadHeader();
                    Converters cons = new Converters();
                    string[] headers = csv.Context.HeaderRecord;
                    csv.Read();
                    tr.Min = cons.ConvertUTCtoDate(csv[0]);
                };
            }

            // Get maximum possible datetime.
            var lastYearOfRecTypePath = Directory.EnumerateDirectories(recTypePath).Last();
            var lastRecFileOfRecTypePath = Directory.EnumerateFiles(lastYearOfRecTypePath).Last();
            path = lastRecFileOfRecTypePath;
            if (File.Exists(path))
            {
                using (TextReader textReader = new StreamReader(File.OpenRead(path)))
                {
                    CsvReader csv = new CsvReader(textReader);
                    csv.Read();
                    csv.ReadHeader();
                    Converters cons = new Converters();
                    string utc = "";
                    //TODO: Lazily get the last record.
                    while (csv.Read())
                    {
                        utc = csv[0];
                    }
                    tr.Max = cons.ConvertUTCtoDate(utc);

                };
            }
            return tr.IsValid();
        }
    }
}