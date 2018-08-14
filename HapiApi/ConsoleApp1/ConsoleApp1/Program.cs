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
            xdoc.Load(@"C: \Users\blaine.harris\Documents\Github\FTECS\HapiApi\WebApi_v1\WebApi_v1\HapiXml\HapiConfiguration.xml");

            XmlNodeList spacecraft = xdoc.GetElementsByTagName("spacecraft")[0].ChildNodes;

            Console.ReadKey();
        }

 
    }
}
