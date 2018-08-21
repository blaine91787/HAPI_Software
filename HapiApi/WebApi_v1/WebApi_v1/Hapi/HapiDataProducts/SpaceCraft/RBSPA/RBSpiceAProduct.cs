using CsvHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using WebApi_v1.Hapi.DataProducts.SpaceCraft.RBSPA.RBSpice.Auxiliary;
using WebApi_v1.Hapi.Utilities;

namespace WebApi_v1.Hapi.DataProducts.SpaceCraft.RBSPA
{
    public class RBSpiceAProduct : DataProduct
    {
        private string _basepath = @"RBSPA\";

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
        public override void Configure(Configuration hapi)
        {
            if (hapi != null)
                HapiConfig = hapi;
            else
                throw new ArgumentNullException(nameof(hapi));

            _basepath = HapiConfig.Basepath + _basepath;

            if(!Directory.Exists(_basepath))
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
                            "rbsp-a-rbspice_lev-0_Auxil_{0}_v1.1.1-00.csv.gz",
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
            string recType = String.Empty;
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
            string recTypePath = Directory.EnumerateDirectories(path).First();

            // We now have the path to the level and record type the user requested.
            tr.Min = GetMinTime(recTypePath);
            tr.Max = GetMaxTime(recTypePath);

            return tr.IsValid();
        }


        private DateTime GetMinTime(string path)
        {
            // Now get the minimum possible time possible.
            // TODO: BUG, .First() will give null if nothing is found and throw an exception
            string firstYearOfRecTypePath = Directory.EnumerateDirectories(path).First();
            string firstRecFileOfRecTypePath = Directory.GetFiles(firstYearOfRecTypePath, "*.gz").First();
            path = firstRecFileOfRecTypePath;

            DateTime minTime = default (DateTime);
            if (File.Exists(path))
            {
                // Decompress *.csv.gz file so we get a *.csv and use CsvReader to get the min time of data available.
                FileInfo fileToDecompress = new FileInfo(path);
                using (FileStream originalFileStream = fileToDecompress.OpenRead())
                using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                using (TextReader decompressedFileReader = new StreamReader(decompressionStream))
                {
                    CsvReader csv = new CsvReader(decompressedFileReader);
                    csv.Read();
                    csv.ReadHeader();
                    Converters cons = new Converters();
                    string[] headers = csv.Context.HeaderRecord;
                    csv.Read();
                    minTime = cons.ConvertUTCtoDate(csv[0]);
                };
            }

            return minTime;
        }

        private DateTime GetMaxTime(string path)
        {
            // Get maximum possible datetime.
            // TODO: BUG, .Last()  give null if nothing is found and throw an exception
            string lastYearOfRecTypePath = Directory.EnumerateDirectories(path).Last();
            string lastRecFileOfRecTypePath = Directory.GetFiles(lastYearOfRecTypePath, "*.gz").Last();
            path = lastRecFileOfRecTypePath;

            DateTime maxTime = default(DateTime);
            if (File.Exists(path))
            {
                // Decompress *.csv.gz file so we get a *.csv and use CsvReader to get the max time of data available.
                FileInfo fileToDecompress = new FileInfo(path);
                using (FileStream originalFileStream = fileToDecompress.OpenRead())
                using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                using (TextReader decompressedFileReader = new StreamReader(decompressionStream))
                {
                    CsvReader csv = new CsvReader(decompressedFileReader);
                    csv.Read();
                    csv.ReadHeader();
                    Converters cons = new Converters();
                    string utc = "";
                    //TODO: Lazily get the last record.
                    while (csv.Read())
                    {
                        utc = csv[0];
                        //Debug.WriteLine(utc);
                    }
                    maxTime = cons.ConvertUTCtoDate(utc);

                };
            }

            return maxTime;
        }
    }
}