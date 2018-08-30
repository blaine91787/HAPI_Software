using CsvHelper;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

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
            max = this.Max = GetMaxTime(path);
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