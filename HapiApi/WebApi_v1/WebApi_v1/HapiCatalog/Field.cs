using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WebApi_v1.HapiCatalog
{
    public class Field
    {
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; }

        public void GetField(XmlElement fieldElement)
        {
            if (fieldElement == null)
                throw new ArgumentNullException("Field's fieldElement is null.");

            if (fieldElement.HasAttribute("name"))
                Name = fieldElement.Attributes["name"].Value;

            Attributes = new Dictionary<string, string>();
            foreach (XmlAttribute attribute in fieldElement.Attributes)
            {
                Attributes.Add(attribute.Name, attribute.Value);
            }
        }
    }
}
