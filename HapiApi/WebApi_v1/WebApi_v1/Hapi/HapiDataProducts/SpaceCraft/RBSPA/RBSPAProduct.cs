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
using System.Text;
using WebApi_v1.HAPI.DataProducts.SpaceCraft.RBSPA.RBSpice.TOFxEH;

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
        public bool InTimeRange(FileInfo fi, DateTime mintime, DateTime maxtime)
        {
            string[] fileNameChunks = fi.Name.Split('_');

            char[] dateArr = default(char[]);
            foreach (string chunk in fileNameChunks)
            {
                if (Int32.TryParse(chunk, out int x))
                {
                    if (x >= 20000000 && x < 30000000)
                        dateArr = chunk.ToCharArray();
                }
            }
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i<dateArr.Length; i++)
            {
                if(i==4 || i==6 || i==8)
                {
                    sb.Append('-');
                }
                sb.Append(dateArr[i]);
            }

            DateTime dt = default(DateTime);
            DateTime.TryParse(sb.ToString(), out dt);


            return ((dt.Date >= mintime.Date && dt < maxtime.Date) || (dt.Date == mintime.Date && dt.Date == maxtime.Date)) ? true : false; //TODO: dt is supposed to be min inclusive and max exclusive
        }
        /// <summary>
        /// 
        /// </summary>
        public override void GetPaths()
        {
            DateTime mintime = Hapi.Properties.TimeRange.UserMin;
            DateTime maxtime = Hapi.Properties.TimeRange.UserMax;
            DateTime mindate = mintime.Date;
            DateTime maxdate = maxtime.Date;
            string filepath = String.Empty;
            string year = String.Empty;
            string filename = String.Empty;
            string filetype = Hapi.Catalog.GetProduct(Hapi.Properties.ID).FileType;
            if (filetype == "cdf")
                filetype = "*.cdf";
            else if (filetype == "csv.gz" || filetype == ".gz")
                filetype = "*csv.gz";

            List<FileInfo> filePathList = new List<FileInfo>();
            foreach (string dir in Directory.GetDirectories(_basepath))
            {
                foreach (string path in Directory.GetFiles(dir, filetype))
                {
                    if (!File.Exists(path))
                        continue;
                    FileInfo fi = new FileInfo(path);
                    if ((InTimeRange(fi, mintime, maxtime)))
                        filePathList.Add(fi);

                }
            }

            Files = filePathList;
            //}

            //while (mintime <= maxtime)
            //{
            //    switch (Hapi.Properties.Product)
            //    {
            //        case ("auxil"):
            //            filename = String.Format(
            //                "rbsp-a-rbspice_lev-0_Auxil_{0}_v1.1.1-00.csv.gz",
            //                mindate.ToString("yyyyMMdd")
            //            );
            //            break;
            //        case ("tofxeh"):
            //            filename = String.Format(
            //                "rbsp-a-rbspice_lev-3-PAP_Auxil_{0}_v1.1.1-00.csv.gz",
            //                mindate.ToString("yyyyMMdd")
            //            );
            //            break;
            //        //case ("auxil"):
            //        //    filename = String.Format(
            //        //        "rbsp-a-rbspice_lev-0_Auxil_{0}_v1.1.1-00.csv.gz",
            //        //        mindate.ToString("yyyyMMdd")
            //        //    );
            //        //    break;
            //        //case ("auxil"):
            //        //    filename = String.Format(
            //        //        "rbsp-a-rbspice_lev-0_Auxil_{0}_v1.1.1-00.csv.gz",
            //        //        mindate.ToString("yyyyMMdd")
            //        //    );
            //        //    break;
            //        //case ("auxil"):
            //        //    filename = String.Format(
            //        //        "rbsp-a-rbspice_lev-0_Auxil_{0}_v1.1.1-00.csv.gz",
            //        //        mindate.ToString("yyyyMMdd")
            //        //    );
            //        //    break;
            //        //case ("auxil"):
            //        //    filename = String.Format(
            //        //        "rbsp-a-rbspice_lev-0_Auxil_{0}_v1.1.1-00.csv.gz",
            //        //        mindate.ToString("yyyyMMdd")
            //        //    );
            //        //    break;
            //        //case ("auxil"):
            //        //    filename = String.Format(
            //        //        "rbsp-a-rbspice_lev-0_Auxil_{0}_v1.1.1-00.csv.gz",
            //        //        mindate.ToString("yyyyMMdd")
            //        //    );
            //        //    break;
            //        default:
            //            break;
            //    }

            //    year = mindate.ToString("yyyy");
            //    filepath = String.Format(_basepath + @"{0}\{1}", year, filename);

            //    if(File.Exists(filepath))
            //        Paths.Add(filepath);

            //    mintime = mintime.AddDays(1.0);
            //    mindate = mintime.Date;
            //}
        }
        /// <summary>
        /// 
        /// TODO: change name to getRecords? Might be confusing with csvhelper though.
        /// </summary>
        /// <returns></returns>
        public override bool GetProduct()
        {
            switch(Hapi.Properties.Product)
            {
                case ("auxil"):
                    AuxiliaryRecords aux = new AuxiliaryRecords(Hapi);
                    Records = aux.GetRecords(Files);
                    break;
                case ("tofxeh"):
                    TofxehRecords eh = new TofxehRecords(Hapi);
                    Records = eh.GetRecords(Files);
                    break;

            }

            return Records.Count() != 0 ? true : false; // TODO: Value can't be null when there's no data. BUG
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