using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleApp1
{
    public class Spacecraft
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Dictionary<string, Instrument> Instruments { get; set; }

        

        public void GetSpacecraft(XmlElement scElement, string basepath)
        {
            if (scElement == null)
                throw new ArgumentNullException("XmlElement is null.");

            if (!Directory.Exists(basepath))
                throw new ArgumentOutOfRangeException("basepath directory does not exist");


            if (scElement.Attributes["id"] != null)
            {
                string id = scElement.Attributes["id"].Value;
                Name = id;
                Console.WriteLine(id);
            }

            if (scElement.Attributes["path"] != null)
            {
                string scPath = scElement.Attributes["path"].Value;
                basepath = scPath.Replace("$data$", basepath);
                Path = basepath;
                Console.WriteLine(basepath);
            }

            // For each instrument node parse and create instrument object, add to  
            XmlNodeList instrumentNodes = scElement.ChildNodes;
            Instruments = new Dictionary<string, Instrument>();
            foreach (XmlNode instrumentNode in instrumentNodes)
            {
                Instrument instrument = new Instrument();
                if(instrumentNode.GetType() == typeof(XmlElement))
                {
                    instrument.GetInstrument((XmlElement)instrumentNode, basepath);
                }
                Instruments.Add(instrument.Name, instrument);
            }
        }
    }
}
