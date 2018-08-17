using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ConsoleApp1.HapiCatalog
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public Dictionary<string, Field> Fields { get; set; }



        public void GetProduct(XmlElement productElement, string basepath)
        {
            if (productElement == null)
                throw new ArgumentNullException("XmlElement is null.");

            if (!Directory.Exists(basepath))
                throw new ArgumentOutOfRangeException("basepath directory does not exist");

            if (productElement.HasAttribute("id"))
                Id = productElement.Attributes["id"].Value;

            if (productElement.HasAttribute("name"))
                Name = productElement.Attributes["name"].Value;

            if (productElement.HasAttribute("description"))
                Description = productElement.Attributes["description"].Value;

            if (productElement.HasAttribute("path"))
            {
                Path = productElement.Attributes["path"].Value;
                if (Path.Contains("$data$"))
                {
                    Path = Path.Replace("$data$", basepath);
                    basepath = Path;
                }
            }

            XmlNodeList fieldNodes = productElement.ChildNodes;
            Fields = new Dictionary<string, Field>();
            foreach (XmlNode fieldNode in fieldNodes)
            {
                Field field = new Field();
                if (fieldNode.GetType() == typeof(XmlElement))
                {
                    field.GetField((XmlElement)fieldNode);
                }
                Fields.Add(field.Name, field);
            }
        }
    }
}
