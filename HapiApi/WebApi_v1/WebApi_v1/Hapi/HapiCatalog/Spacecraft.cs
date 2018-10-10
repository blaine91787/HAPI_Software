using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace WebApi_v1.HAPI.Catalog
{
    public class Spacecraft
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Dictionary<string, Instrument> Instruments { get; set; }

        /// <summary>
        /// Creates a Spacecraft object from the spacecraft elements in HapiCatalog.xml
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when XmlElement scElement argument is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when basepath does not </exception>
        /// <param name="scElement">The HapiCatalog.xml spacecraft element.</param>
        /// <param name="basepath">The path of the directory which contains the spacecraft dirs.</param>
        public void GetSpacecraft(XmlElement scElement, string basepath)
        {
            if (scElement == null)
                throw new ArgumentNullException("XmlElement scElement is null.");

            if (!Directory.Exists(basepath))
                throw new ArgumentOutOfRangeException("basepath directory does not exist");

            if (!scElement.HasAttributes)
                throw new InvalidOperationException("Spacecraft must have attributes for catalog entry to be valid. Check the catalog xml for errors.");

            foreach (XmlAttribute attr in scElement.Attributes)
            {

                switch (attr.Name)
                {
                    case ("name"):
                        Name = attr.Value;
                        break;

                    case ("path"):
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

            // For each instrument node parse and create instrument object, add to Instruments dictionary  
            XmlNodeList instrumentNodes = scElement.ChildNodes;
            Instruments = new Dictionary<string, Instrument>();
            foreach (XmlNode instrumentNode in instrumentNodes)
            {
                Instrument instrument = new Instrument();

                if (instrumentNode.GetType() == typeof(XmlElement))
                    instrument.GetInstrument((XmlElement)instrumentNode, basepath);

                Instruments.Add(instrument.Name, instrument);
            }
        }
    }
}
