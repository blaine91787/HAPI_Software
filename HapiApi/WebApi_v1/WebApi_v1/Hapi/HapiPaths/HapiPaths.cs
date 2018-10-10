using System;
using System.IO;

namespace WebApi_v1.HAPI
{
    public class HapiPaths
    {
        public string UserPath { get; set; } = String.Empty;
        public string SoftwarePath { get; set; } = String.Empty;
        public string DataPath { get; set; } = String.Empty;
        public string CatalogXmlPath { get; set; } = String.Empty;
        public string ConfigurationXmlPath { get; set; } = String.Empty;

        public void ResolvePaths()
        {
            UserPath = Hapi.Registry.UserPath;

            if (!Directory.Exists(UserPath))
                throw new DirectoryNotFoundException("RBSPAProduct._basepath could not resolve to a valid path.");

            SoftwarePath = UserPath + @"\Documents\Github\FTECS\HapiApi\WebApi_v1\";
            CatalogXmlPath = SoftwarePath + @"\WebApi_v1\\Hapi\HapiXml\HapiCatalog.xml";
            ConfigurationXmlPath = SoftwarePath + @"\WebApi_v1\Hapi\HapiXml\HapiConfiguration.xml";

            DataPath = Hapi.Registry.DataPath;

            if (Hapi.Registry.UseFtecsData.ToLower() == "true" && Directory.Exists(DataPath))
                DataPath = @"\\\\\\\\" + DataPath; // HACK: Need to figure out how to keep slashes under control. Effects HapiCatalog.Create
            else
                DataPath = Hapi.Registry.TestDataPath;

            VerifyPathsExist();
        }

        private void VerifyPathsExist()
        {
            if (!Directory.Exists(UserPath))
                throw new DirectoryNotFoundException("The user path could not be found.");

            if (!Directory.Exists(SoftwarePath))
                throw new DirectoryNotFoundException("The software path could not be found.");

            if (!File.Exists(CatalogXmlPath))
                throw new DirectoryNotFoundException("The catalog xml path could not be found.");

            if (!File.Exists(ConfigurationXmlPath))
                throw new DirectoryNotFoundException("The configuration xml path could not be found.");

            if (!Directory.Exists(DataPath))
                throw new DirectoryNotFoundException("The data path could not be found.");
        }
    }
}