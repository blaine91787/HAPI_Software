using System.IO;
using System.Linq;
using System.Xml;

namespace WebApi_v1.HAPI.Utilities
{
    public class HapiXmlReader
    {

        public void LoadHapiSpecs(string configurationXmlPath, out string version, out string[] capabilities, out string[] endpoints, out string dataArchivePath, out string catalogPath)
        {
            if (!File.Exists(configurationXmlPath))
                throw new FileNotFoundException("Hapi Configuration Xml not found.");

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(configurationXmlPath);

            version = xdoc.SelectSingleNode("/HapiConfiguration/Version").InnerText;
            capabilities = xdoc.SelectSingleNode("/HapiConfiguration/Capabilities").InnerText.Split(',').ToArray();
            endpoints = xdoc.SelectSingleNode("/HapiConfiguration/Endpoints").InnerText.Split(',').ToArray();
            dataArchivePath = xdoc.SelectSingleNode("/HapiConfiguration/DataArchivePath").InnerText;
            catalogPath = xdoc.SelectSingleNode("/HapiConfiguration/CatalogPath").InnerText;
        }
    }
}