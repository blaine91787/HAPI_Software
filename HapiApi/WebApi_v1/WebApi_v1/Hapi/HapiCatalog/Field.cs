using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WebApi_v1.HAPI
{
    public class Field
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Units { get; set; }
        public string Fill { get; set; }
        public string Length { get; set; }
        public string Description { get; set; }
        public string Bins { get; set; }
        public string Size { get; set; }



        public void GetField(XmlElement fieldElement)
        {
            if (fieldElement == null)
                throw new ArgumentNullException("Field's fieldElement is null."); ;

            if (!fieldElement.HasAttributes)
                throw new InvalidOperationException("Fields must have attributes for catalog entry to be valid. Check the catalog xml for errors."); ;

            foreach (XmlAttribute attr in fieldElement.Attributes)
            {
                switch (attr.Name)
                {
                    case ("name"):
                        Name = attr.Value.ToLower();
                        break;
                    case ("type"):
                        Type = attr.Value.ToLower();
                        break;
                    case ("units"):
                        Units = attr.Value.ToLower();
                        break;
                    case ("fill"):
                        Fill = attr.Value.ToLower();
                        break;
                    case ("length"):
                        Length = attr.Value.ToLower();
                        break;
                    case ("description"):
                        Description = attr.Value.ToLower();
                        break;
                    case ("bins"):
                        Bins = attr.Value.ToLower();
                        break;
                    case ("size"):
                        Size = attr.Value.ToLower();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(attr.Name, "Not a valid field attribute.");
                }
            }
        }
    }
}
