using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WebApi_v1.HapiCatalog
{
    public class Catalog
    {
        public Dictionary<string, Spacecraft> Spacecrafts { get; set; }

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
                        Spacecrafts = new Dictionary<string, Spacecraft>();
                        XmlNodeList spacecraftNodes = catalogNode.ChildNodes;

                        foreach (XmlNode spacecraftNode in spacecraftNodes)
                        {
                            Spacecraft sc = new Spacecraft();
                            if (spacecraftNode.GetType() == typeof(XmlElement))
                            {
                                sc.GetSpacecraft((XmlElement)spacecraftNode, basepath);
                            }
                            Spacecrafts.Add(sc.Name, sc);
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Spacecraft sc in Spacecrafts.Values)
            {
                foreach (Instrument instr in sc.Instruments.Values)
                {
                    foreach (Product prod in instr.Products.Values)
                    {
                        sb.AppendFormat("{0} : {1}\n", prod.Name, prod.Path);
                    }
                }
            }
            return sb.ToString();
        }

        //private HapiCatalog.Catalog _catalog;

        //public HapiCatalog.Catalog CatalogStr
        //{
        //    get
        //    {
        //        if (_catalog == null)
        //        {
        //            _catalog = new HapiCatalog.Catalog();
        //            _catalog.CreateCatalog();
        //            return _catalog;
        //        }
        //        else
        //        {

        //            return _catalog;
        //        }
        //    }
        //}
    }
}
