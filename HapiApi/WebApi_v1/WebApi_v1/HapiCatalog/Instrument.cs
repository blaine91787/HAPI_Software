using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace WebApi_v1.HapiCatalog
{
    public class Instrument
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Dictionary<string, Product> Products { get; set; }

        public void GetInstrument(XmlElement instrumentElement, string basepath)
        {
            if (instrumentElement == null)
                throw new ArgumentNullException("XmlElement is null.");

            if (!Directory.Exists(basepath))
                throw new ArgumentOutOfRangeException("basepath directory does not exist");

            if (instrumentElement.HasAttributes)
            {
                foreach (XmlAttribute attr in instrumentElement.Attributes)
                {
                    switch (attr.Name)
                    {
                        case "name":
                            Name = attr.Value;
                            break;
                        case "path":
                            Path = attr.Value;
                            break;
                    }
                }
            }

            XmlNodeList productNodes = instrumentElement.ChildNodes;
            Products = new Dictionary<string, Product>();
            foreach (XmlNode productNode in productNodes)
            {
                Product product = new Product();
                if (productNode.GetType() == typeof(XmlElement))
                {
                    product.GetProduct((XmlElement)productNode, basepath);
                }
                Products.Add(product.Name, product);
            }
        }
    }
}