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
            string[] productId = userInput.Split('_');

            HapiCatalog.Product prod = hapiConfig.Catalog.Spacecrafts[productId[0]].Instruments[productId[1]].Products[productId[2]];

            Console.WriteLine(String.Format("{0}\n{1}\n{2}", prod.Id, prod.Name, prod.Path));

            foreach(var field in prod.Fields)
            {
                Console.WriteLine(field.Value.Name);
                foreach(var attr in field.Value.Attributes)
                {
                    Console.WriteLine(String.Format("{0} : {1}", attr.Key, attr.Value));
                }
            }
            Console.ReadKey();
        } 
    }
}
