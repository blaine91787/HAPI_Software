using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WebApi_v1.HAPI.Catalog
{
    public class HapiCatalog
    {
        public Dictionary<string, Spacecraft> Spacecrafts { get; set; }

        public void Create()
        {
            string catalogXmlPath = @"C:\Users\blaine.harris\Documents\Github\FTECS\HapiApi\WebApi_v1\WebApi_v1\Hapi\HapiXml\HapiCatalog.xml";
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
            if (productId.Split('_').Length != 3)
                return null;

            Spacecraft sc = null;
            Instrument instr = null;

            string[] ids = productId.Split('_');
            string scID = ids[0];
            string instrID = ids[1];
            string prodID = ids[2];

            if (!Spacecrafts.Keys.Contains(scID))                
                return null;

            // TODO: Exceptions shouldn't be thrown. Instead it should be handled gracefully and reported to user.
            sc = Spacecrafts[scID];
            if (!sc.Instruments.Keys.Contains(instrID))
                return null;

            instr = sc.Instruments[instrID];
            if (!instr.Products.Keys.Contains(prodID))
                return null;

            return instr.Products[prodID];
        }

        public List<Product> GetProducts()
        {
            if (Spacecrafts == null)
                throw new InvalidOperationException("Catalog must be created before you can get the product.");

            List<Product> prods = new List<Product>();
            {
                foreach (Spacecraft sc in Spacecrafts.Values)
                {
                    foreach (Instrument instr in sc.Instruments.Values)
                    {
                        foreach (Product prod in instr.Products.Values)
                        {
                            prods.Add(prod);
                        }
                    }
                }
                return prods;
            }
        }

        public bool IsValidProduct(string productID)
        {
            return GetProduct(productID) != null ? true : false;
        }
    }
}
