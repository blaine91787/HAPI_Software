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
            HapiConfiguration hapiConfig = new HapiConfiguration();
            string userInput = "rbspa_rbspice_tofxeh";
            string[] id = userInput.Split('_');
            string idSC = id[0];
            string idInstr = id[1];
            string idProd = id[2];

            foreach (HapiCatalog.Product prod in hapiConfig.Catalog.Spacecrafts[idSC].Instruments[idInstr].Products.Values)
            {
                Console.WriteLine(prod.ToString());
            }

            HapiCatalog.Product prod1 = hapiConfig.Catalog.GetProduct(userInput);

            Console.WriteLine(prod1.ToString());

            foreach(HapiCatalog.Product prod2 in hapiConfig.Catalog.GetProducts(idSC + "_" + idInstr))
            {
                Console.WriteLine(prod2.ToString());
            }
            Console.ReadKey();
        } 
    }
}
