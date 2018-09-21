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
using CDF;

namespace ConsoleApp1
{
    public class CDFTesting
    {
        public static void Main(string[] args)
        {

            //WebRequest wr = new WebRequest();
            //wr.Run();

            //string path1 = @"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH\2013\rbsp-a-rbspice_lev-3-PAP_TOFxEH_20130126_v1.1.2-00.cdf";
            //string path2 = @"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH\2013\rbsp-a-rbspice_lev-3-PAP_TOFxEH_20130127_v1.1.2-00.cdf";

            ////CDFReader cdf1 = new CDFReader(path1);
            ////cdf1.Close();

            //CDFReader cdf2 = new CDFReader(path2);
            //cdf2.Close();

            string[] arg = new string[]
            {
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\2013",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH\2013",
                //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH\2013",
                @"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH",
                //@"C:\HapiApi",
                //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_0\Auxil\2012",
                //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_0",
                //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_1",
                //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_2",
                //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3",
                //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP",
            };

            CDFTester cdfT = new CDFTester();
            Stopwatch sw = Stopwatch.StartNew();
            cdfT.Run(args);
            sw.Stop();
            long get = sw.ElapsedMilliseconds;


            //Console.ReadKey();

        }


    }
}
