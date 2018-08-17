using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleApp1.HapiCatalog
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

        public Product GetProduct(string productId)
        {
            if (Spacecrafts == null)
                throw new InvalidOperationException("Catalog must be created before you can get the product.");

            // Must have format: [spacecraft]_[instrument]_[product]
            Spacecraft sc = null;
            Instrument instr = null;

            string[] ids = productId.Split('_');
            if (!Spacecrafts.Keys.Contains(ids[0]))
                return null;

            sc = Spacecrafts[ids[0]];
            if (!sc.Instruments.Keys.Contains(ids[1]))
                return null;

            instr = sc.Instruments[ids[1]];
            if (!instr.Products.Keys.Contains(ids[2]))
                return null;

            return instr.Products[ids[2]];
        }

        public List<Product> GetProducts(string instrumentId)
        {
            if(Spacecrafts == null)
                throw new InvalidOperationException("Catalog must be created before you can get the product.");

            //Must have format: [spacecraft]_[instrument]
            Spacecraft sc = null;

            string[] ids = instrumentId.Split('_');
            if (!Spacecrafts.Keys.Contains(ids[0]))
                return null;

            sc = Spacecrafts[ids[0]];
            if (!sc.Instruments.Keys.Contains(ids[1]))
                return null;

            List<Product> prods = new List<Product>();
            foreach(Product prod in sc.Instruments[ids[1]].Products.Values)
            {
                prods.Add(prod);
            }

            return prods;
        }
    }
}
