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
            xdoc.Load(@"C:\Users\blaine.harris\Documents\Github\FTECS\HapiApi\WebApi_v1\WebApi_v1\HapiDataProducts\HapiCatalog.xml");

            XmlNodeList spacecraft = xdoc.SelectNodes("/catalog/missions/spacecraft");

            foreach(XmlElement sc in spacecraft)
            {
                if(sc.HasChildNodes) // check for spacecraft name
                {
                    XmlNodeList  = sc.ChildNodes;

                    if(scname.HasChildNodes) // check for spacecraft levels available
                    {
                        XmlElement levels = scname.FirstChild;
                        foreach (XmlElement product in levels)
                        {
                            Console.WriteLine(product.Name);
                        }
                    }

                    //if(type.HasChildNodes)
                    //{
                    //    foreach (XmlElement param in type)
                    //    {
                    //        Console.WriteLine(param.InnerText);
                    //    }
                    //}
                }
                //Console.WriteLine(node);
            }

            Console.ReadKey();
        }

 
    }
}
