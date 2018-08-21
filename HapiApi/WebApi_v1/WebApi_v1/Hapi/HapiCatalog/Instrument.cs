using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace WebApi_v1.HAPI.Catalog
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

            if (!instrumentElement.HasAttributes)
                throw new InvalidOperationException("Instruments must have attributes for catalog entry to be valid. Check the catalog xml for errors.");

            foreach (XmlAttribute attr in instrumentElement.Attributes)
            {
                switch (attr.Name)
                {
                    case "name":
                        Name = attr.Value;
                        break;
                    case "path":
                        Path = attr.Value;
                        if (Path.Contains("$data$"))
                        {
                            Path = Path.Replace("$data$", basepath).Replace(@"\\", @"\");
                            basepath = Path;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(attr.Name, "Not a valid field attribute.");
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
                Products.Add(product.Id, product);
            }
        }
    }
}