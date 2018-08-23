using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ConsoleApp1;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {




            RunCatalog();


            Console.ReadKey();
        }

        static void RunCatalog()
        {
            HapiConfiguration hapiConfig = new HapiConfiguration();
            string userInput = "rbspa_rbspice_auxil";
            string[] id = userInput.Split('_');
            string scId = id[0];
            string instrId = id[1];
            string prodId = id[2];

            foreach (HapiCatalog.Product prod in hapiConfig.Catalog.Spacecrafts[scId].Instruments[instrId].Products.Values)
            {
                Console.WriteLine(prod.ToString());
            }

            HapiCatalog.Product prod1 = hapiConfig.Catalog.GetProduct(userInput);

            Console.WriteLine(prod1.ToString());

            foreach (HapiCatalog.Product prod2 in hapiConfig.Catalog.GetProducts(scId + "_" + instrId))
            {
                Console.WriteLine(prod2.ToString());
            }
        }
    }
}
