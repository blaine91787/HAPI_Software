using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleApp1
{
    public partial class Hapi
    {
        public Dictionary<string, Spacecraft> Catalog { get; set; }

        public void CreateCatalog()
        {
            string catalogXmlPath = @"C:\Users\blaine.harris\Documents\Github\FTECS\HapiApi\WebApi_v1\WebApi_v1\HapiXml\HapiCatalog.xml";
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreComments = true;
            using (XmlReader reader = XmlReader.Create(catalogXmlPath, readerSettings))
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(reader);

                XmlNodeList catalogNodes = xdoc.GetElementsByTagName("catalog");
                foreach (XmlNode catalogNode in catalogNodes)
                {
                    string basepath = String.Empty;
                    if (catalogNode.Attributes["path"] != null)
                        basepath = catalogNode.Attributes["path"].Value;

                    if (catalogNode.HasChildNodes)
                    {
                        Catalog = new Dictionary<string, Spacecraft>();
                        XmlNodeList spacecraftNodes = catalogNode.ChildNodes;

                        foreach(XmlNode spacecraftNode in spacecraftNodes)
                        {
                            Spacecraft sc = new Spacecraft();
                            if (spacecraftNode.GetType() == typeof(XmlElement))
                            {
                                sc.GetSpacecraft((XmlElement)spacecraftNode, basepath);

                            }
                            Catalog.Add(sc.Name, sc);
                        }
                    }
                }
            }
            foreach(Spacecraft sc in Catalog.Values)
            {
                Console.WriteLine(sc.Name + " : " + sc.Path);
                foreach(Instrument instr in sc.Instruments.Values)
                {
                    Console.WriteLine(instr.Name + " : " + instr.Path);
                    foreach(HapiProduct prod in instr.Products.Values)
                    {
                        Console.WriteLine(prod.Id + " : " + prod.Path);
                        foreach(Field field in prod.Fields.Values)
                        {
                            Console.WriteLine(field.Name);
                        }
                    }
                }
            }
        }
    }
}
