using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleApp1
{
    public class HapiProduct
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public Dictionary<string, Field> Fields { get; set; }



        public void GetProduct(XmlElement productElement, string basepath)
        {
            if (productElement == null)
                throw new ArgumentNullException("XmlElement is null.");

            if (!Directory.Exists(basepath))
                throw new ArgumentOutOfRangeException("basepath directory does not exist");

            if (productElement.HasAttribute("id"))
            {
                string id = productElement.Attributes["id"].Value;
                Id = id;
                Console.WriteLine(id);
            }

            if(productElement.HasAttribute("path"))
            {
                string productPath = productElement.Attributes["path"].Value;
                basepath = productPath.Replace("$data$", basepath);
                Path = basepath;
                Console.WriteLine(Path);
            }

            XmlNodeList fieldNodes = productElement.ChildNodes;
            Fields = new Dictionary<string, Field>();
            foreach(XmlNode fieldNode in fieldNodes)
            {
                Field field = new Field();
                if(fieldNode.GetType() == typeof(XmlElement))
                {
                    field.GetField((XmlElement)fieldNode);
                }
                Fields.Add(field.Name, field);
            }
        }
    }
}
