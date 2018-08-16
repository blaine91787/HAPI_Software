using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleApp1
{
    public class Field
    {
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; }

        public void GetField(XmlElement fieldElement)
        {
            if (fieldElement == null)
                throw new ArgumentNullException("XmlElement is null.");

            if (fieldElement.HasAttribute("name"))
            {
                string name = fieldElement.Attributes["name"].Value;
                Name = name;
                Console.WriteLine(Name);
            }
        }
    }
}
