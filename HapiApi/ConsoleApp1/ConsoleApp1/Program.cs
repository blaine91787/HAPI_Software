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

namespace ConsoleApp1
{
    public class Program
    {
        public static void Main()
        {

            HapiCatalogProducer catProducer = new HapiCatalogProducer();
            catProducer.CreateCatalog();
            //string cdfFilePath = @"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH\2013\rbsp-a-rbspice_lev-3-PAP_TOFxEH_20130126_v1.1.2-00.cdf";
            //CDFReader cdfFile = new CDFReader(cdfFilePath);
            //cdfFile.GetAttributes();
            //Console.ReadKey();
        }



    }
}
