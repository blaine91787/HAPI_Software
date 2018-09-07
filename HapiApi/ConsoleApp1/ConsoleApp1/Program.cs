using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ConsoleApp1;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SPDF.CDF.CSharp;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApi_v1.HAPI.Utilities;
using System.Threading;

namespace ConsoleApp1
{
    public class Program
    {


        public static void Initialize(CDFReader cdf, DateTime start, DateTime stop, string[] parameters, out List<int> indeces, out string[] headers)
        {
            List<int> indecesTemp = new List<int>();
            List<string> headersTemp = new List<string>();
            CDF_Variable utcVar = cdf.GetVariable("UTC");

            Converters con = new Converters();
            for(int i = 0; i<utcVar.Records; i++)
            { 
                DateTime utc = con.ConvertUTCtoDate(utcVar[i].ToString());
                if (utc >= start && utc < stop)
                    indecesTemp.Add(i);
            }

            List<CDF_Variable> vars = cdf.GetListOfVariables(parameters);

            foreach (CDF_Variable var in vars)
            {
                headersTemp.Add(var.Name);
            }

            indeces = indecesTemp;
            headers = headersTemp.ToArray();
        }

        public static List<string> GetHeader(CDFReader cdf)
        {
            List<string> temp = new List<string>();

            return temp;
        }

        public static void Main()
        {
            Converters con = new Converters();
            string t = "2012-318T22:14:58.645";
            string p = "2012-318T22:31:21.411";
            DateTime start = con.ConvertUTCtoDate(t);
            DateTime stop = con.ConvertUTCtoDate(p);
            CDFReader cdf = new CDFReader(@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\ESRHELT\2012\rbsp-a-rbspice_lev-3-PAP_ESRHELT_20121113_v1.1.1-01.cdf");
            string[] parameters = new string[] {
                "epoch",
                "utc",
                "ddoy",
                "et",
                "midet"
            };

            List<Dictionary<string, string>> records = new List<Dictionary<string, string>>();
            List<int> indeces;
            string[] header;
            //List<string> product;
            Initialize(cdf, start, stop, parameters, out indeces, out header);

            List<CDF_Variable> varList = cdf.GetListOfVariables(parameters);
            for (int i = 0; i < indeces.ToArray().Length; i++)
            {
                Dictionary<string, string> temp = new Dictionary<string, string>();
                foreach (CDF_Variable var in varList)
                {
                    string str = cdf.GetVariableString(var, indeces[i]);
                    temp.Add(var.Name, str);
                }
                records.Add(temp);
            }

            foreach (string head in header)
                Console.Write(head + ", ");

            Console.WriteLine();

            foreach (Dictionary<string, string> dict in records)
            {
                foreach (KeyValuePair<string, string> pair in dict)
                {
                    Console.Write(pair.Value + ", ");
                }
                Console.WriteLine();
            }
            Console.ReadKey();
        }



    }
}
