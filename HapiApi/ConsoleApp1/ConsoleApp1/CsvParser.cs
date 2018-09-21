using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using CsvHelper;

namespace ConsoleApp1
{
    public static class CsvParser
    {
        public static async Task<bool?> Parse(HttpResponseMessage message)
        {

            using (TextReader responseStream = new StreamReader(await message.Content.ReadAsStreamAsync()))
            {
                CsvReader csv = new CsvReader(responseStream);
                csv.Configuration.BadDataFound = null;


                while (csv.Read())
                {
                    string[] x = csv.Parser.Read();
                    foreach(string v in x)
                    {
                        Console.Write(String.Format(
                            @"{0},", v
                            ));
                    }
                    Console.WriteLine("\n");
                }

                //while (csv.Read())
                //{
                //    string[] rec = csv.Parser.Read();
                //    if (rec == null)
                //        continue;
                //    foreach (string str in rec)
                //    {
                //        Console.Write(str + ",");
                //    }
                //    Console.WriteLine("");
                //}
                csv.Dispose();
            }

            return true;
        }
    }
}
