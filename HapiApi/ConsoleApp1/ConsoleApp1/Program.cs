using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(@"C:\Users\unicornpuke\Documents\Github\FTECS\HapiApi\WebApi_v1\WebApi_v1\HapiDataProducts\HapiCatalog.xml");

            XmlNodeList spacecraft = xdoc.GetElementsByTagName("spacecraft")[0].ChildNodes;

            string t1 = "    ";
            string t2 = "\t";
            string t3 = "\t    ";
            string t4 = "\t\t";

            foreach(XmlElement sc in spacecraft)
            {
                XmlNodeList levels = sc.ChildNodes;
                foreach(XmlElement level in levels)
                {
                    XmlNodeList products = level.ChildNodes;
                    foreach(XmlElement product in products)
                    {
                        Console.WriteLine(sc.Name + "_" + level.Name + "_" + product.Name + ":\n");
                        if(product.GetElementsByTagName("productdescription")[0] != null)
                            Console.WriteLine(t2 + product.GetElementsByTagName("productdescription")[0].Attributes["description"].Value);

                        if (product.GetElementsByTagName("path")[0] != null)
                            Console.WriteLine(t2 + product.GetElementsByTagName("path")[0].Attributes["path"].Value);

                        XmlNodeList fields = product.GetElementsByTagName("field");
                        foreach(XmlElement field in fields)
                        {
                            Console.WriteLine(t3 + field.InnerText);
                        }
                        Console.WriteLine();
                    }
                }
            }

            Console.ReadKey();
        }

 
    }
}
