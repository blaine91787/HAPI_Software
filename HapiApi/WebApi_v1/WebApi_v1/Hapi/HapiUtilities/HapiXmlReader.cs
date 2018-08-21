using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace WebApi_v1.HAPI.Utilities
{
    public class HapiXmlReader
    {

        public void LoadHapiSpecs(out string version, out string[] capabilities, out string[] endpoints, out string dataArchivePath, out string catalogPath)
        {
            string hapiConfigurationXml = @"C:\Users\blaine.harris\Documents\Github\FTECS\HapiApi\WebApi_v1\WebApi_v1\Hapi\HapiXml\HapiConfiguration.xml";

            if (!File.Exists(hapiConfigurationXml))
                throw new FileNotFoundException("Hapi Configuration Xml not found.");

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(hapiConfigurationXml);

            version = xdoc.SelectSingleNode("/HapiConfiguration/Version").InnerText;
            capabilities = xdoc.SelectSingleNode("/HapiConfiguration/Capabilities").InnerText.Split(',').ToArray();
            endpoints = xdoc.SelectSingleNode("/HapiConfiguration/Endpoints").InnerText.Split(',').ToArray();
            dataArchivePath = xdoc.SelectSingleNode("/HapiConfiguration/DataArchivePath").InnerText;

            catalogPath = xdoc.SelectSingleNode("/HapiConfiguration/CatalogPath").InnerText;
        }
    }
}