using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ConsoleApp1
{
    public class Instrument
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Dictionary<string, HapiProduct> Products { get; set; }

        public void GetInstrument(XmlElement instrumentElement, string basepath)
        {
            if (instrumentElement == null)
                throw new ArgumentNullException("XmlElement is null.");

            if (!Directory.Exists(basepath))
                throw new ArgumentOutOfRangeException("basepath directory does not exist");

            if (instrumentElement.Attributes["id"] != null)
            {
                string id = instrumentElement.Attributes["id"].Value;
                Console.WriteLine(id);
                Name = id;
            }

            if (instrumentElement.Attributes["path"] != null)
            {
                string instrumentPath = instrumentElement.Attributes["path"].Value;
                basepath = instrumentPath.Replace("$data$", basepath);
                Console.WriteLine(basepath);
            }

            XmlNodeList productNodes = instrumentElement.ChildNodes;
            Products = new Dictionary<string, HapiProduct>();
            foreach(XmlNode productNode in productNodes)
            {
                HapiProduct product = new HapiProduct();
                if(productNode.GetType() == typeof(XmlElement))
                {
                    product.GetProduct((XmlElement)productNode, basepath);
                }
                Products.Add(product.Id, product);
            }
        }
    }
}