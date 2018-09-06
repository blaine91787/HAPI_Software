using CsvHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using WebApi_v1.HAPI.DataProducts.SpaceCraft.RBSPA.RBSpice.Auxiliary;
using WebApi_v1.HAPI.Configuration;
using WebApi_v1.HAPI.Utilities;
using WebApi_v1.HAPI.Properties;

namespace WebApi_v1.HAPI.DataProducts.SpaceCraft.RBSPA
{
    public class RBSPAProduct : HapiDataProduct
    {
        private string _basepath = String.Empty;

        /// <summary>
        /// 
        /// </summary>
        public override void Configure(Hapi hapi)
        {
            this.Hapi = hapi;

            if (Hapi.Configuration == null)
                throw new ArgumentNullException(nameof(Hapi.Configuration));

            if (Hapi.Properties == null)
                throw new ArgumentNullException(nameof(Hapi.Properties));

            try
            {
                _basepath = Hapi.Catalog.GetProduct(Hapi.Properties.ID).Path;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Unable to get product id's path.", e);
            }

            if(!Directory.Exists(_basepath))
                throw new DirectoryNotFoundException("RBSPAProduct._basepath could not resolve to a valid path.");

            GetPaths();
        }
        /// <summary>
        /// 
        /// </summary>
        public override void GetPaths()
        {
            Paths = new List<string>();
            DateTime mintime = Hapi.Properties.TimeRange.UserMin;
            DateTime maxtime = Hapi.Properties.TimeRange.UserMax;
            DateTime mindate = mintime.Date;
            DateTime maxdate = maxtime.Date;
            string filepath = String.Empty;
            string year = String.Empty;
            string filename = String.Empty;

            while (mintime <= maxtime)
            {
                switch (Hapi.Properties.Product)
                {
                    case ("auxil"):
                        year = mindate.ToString("yyyy");
                        filename = String.Format(
                            "rbsp-a-rbspice_lev-0_Auxil_{0}_v1.1.1-00.csv.gz",
                            mindate.ToString("yyyyMMdd")
                        );
                        break;

                    default:
                        break;
                }

                filepath = String.Format(_basepath + @"{0}\{1}", year, filename);

                if(File.Exists(filepath))
                    Paths.Add(filepath);

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
            AuxiliaryRecords aux = new AuxiliaryRecords(Hapi);
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
            if (Hapi.Properties.TimeRange == null)
                throw new ArgumentNullException(nameof(Hapi.Properties.TimeRange));

            TimeRange tr = Hapi.Properties.TimeRange;
            // We now have the path to the level and record type the user requested.
            if (Hapi.Properties.ID == "rbspa_rbspice_auxil")
                tr.GetAvailableTimeRange(Hapi.Catalog.GetProduct(Hapi.Properties.ID).Path, out _, out _);
            else
                tr.GetAvailableTimeRange(Hapi.Catalog.GetProduct(Hapi.Properties.ID), out _, out _);

            return tr.IsValid();
        }


        private DateTime GetMinTime(string path)
        {
            // Now get the minimum possible time possible.
            // TODO: BUG, .First() will give null if nothing is found and throw an exception
            string firstYearOfRecTypePath = Directory.EnumerateDirectories(path).First();
            string firstRecFileOfRecTypePath = Directory.GetFiles(firstYearOfRecTypePath, "*.gz").FirstOrDefault();
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
            string lastRecFileOfRecTypePath = Directory.GetFiles(lastYearOfRecTypePath, "*.gz").LastOrDefault();
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