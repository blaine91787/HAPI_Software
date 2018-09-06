using CsvHelper;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using WebApi_v1.HAPI.Catalog;

namespace WebApi_v1.HAPI.Utilities
{
    public class TimeRange
    {
        public bool InTimeRange { get; set; }
        public bool STime { get; set; }
        public bool ETime { get; set; }
        public DateTime UserMin { get; set; }
        public DateTime UserMax { get; set; }
        public DateTime Min { get; set; }
        public DateTime Max { get; set; }

        public TimeRange()
        {
            InTimeRange = false;
            UserMin = default(DateTime);
            UserMax = default(DateTime);
            Min = default(DateTime);
            Max = default(DateTime);
        }

        public bool IsValid()
        {
            // TODO: Find what the spec expects for min and max times
            if (UserMin == UserMax)
                return false;

            if ((UserMin.Date >= Min.Date && UserMax.Date <= Max.Date))
            {
                InTimeRange = true;
                return true;
            }
            return false;
        }

        public void GetAvailableTimeRange(string path, out DateTime min, out DateTime max)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException(path);


            min = this.Min = GetMinTime(path);

            if (min == default(DateTime))
                throw new InvalidOperationException("Error calculating min time.");

            max = this.Max = GetMaxTime(path);

            if (max == default(DateTime))
                throw new InvalidOperationException("Error calculating max time.");
        }

        public void GetAvailableTimeRange(Product product, out DateTime min, out DateTime max)
        {
            //if (!Directory.Exists(path))
            //    throw new DirectoryNotFoundException(path);

            min = max = default(DateTime);

            Converters cons = new Converters();


            min = this.Min = cons.ConvertUTCtoDate(product.StartTime);
            max = this.Max = cons.ConvertUTCtoDate(product.StopTime);

            if (min == default(DateTime))
                throw new InvalidOperationException("Error calculating min time."); ;

            if (max == default(DateTime))
                throw new InvalidOperationException("Error calculating max time.");
        }

        private DateTime GetMinTime(string path)
        {
            // Now get the minimum possible time possible.
            // TODO: BUG, .First() will give null if nothing is found and throw an exception
            string firstYearOfRecTypePath = Directory.EnumerateDirectories(path).First();
            string firstRecFileOfRecTypePath = Directory.GetFiles(firstYearOfRecTypePath, "*.gz").FirstOrDefault();
            path = firstRecFileOfRecTypePath;

            DateTime minTime = default(DateTime);
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
            else
            {
                throw new FileNotFoundException("Could not find file to calculate min time.", path);
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