using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WebApi_v1.HAPI.Utilities;

namespace WebApi_v1.HAPI
{
    public class HapiPaths
    {
        private bool UseFtecsArchive = false;
        private bool UseGazelleArchive = false;

        public string UserPath { get; set; } = String.Empty;
        public string SoftwarePath { get; set; } = String.Empty;
        public string DataPath { get; set; } = String.Empty;
        public string CatalogXmlPath { get; set; } = String.Empty;
        public string ConfigurationXmlPath { get; set; } = String.Empty;

        public void ResolveUserPath()
        {
            string unicornpukepath = @"C:\Users\unicornpuke\";
            string thinkpadpath = @"C:\Users\FTECS Account\";
            string gazellepath = @"C:\Users\blaine.harris\";

            if (Directory.Exists(thinkpadpath))
                UserPath = thinkpadpath;
            else if (Directory.Exists(gazellepath))
                UserPath = gazellepath;//gazellepath;
            else if (Directory.Exists(unicornpukepath))
                UserPath = unicornpukepath;
            else
                throw new DirectoryNotFoundException("RBSPAProduct._basepath could not resolve to a valid path.");




            SoftwarePath = UserPath + @"\Documents\Github\FTECS\HapiApi\WebApi_v1\";
            CatalogXmlPath = SoftwarePath + @"\WebApi_v1\\Hapi\HapiXml\HapiCatalog.xml";
            ConfigurationXmlPath = SoftwarePath + @"\WebApi_v1\Hapi\HapiXml\HapiConfiguration.xml";

            // Get Hapi Specification defined defaults.
            HapiXmlReader hxr = new HapiXmlReader();
            string datapath;
            hxr.LoadHapiSpecs(ConfigurationXmlPath, out _, out _, out _, out datapath, out _);
            DataPath = datapath;

            if (UseFtecsArchive)
                DataPath = @"\\\\\\\\" + DataPath;
            else
                DataPath = @"C:\HapiApi\data\Archive\";

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

        public void ResolveDataPath()
        {
            string ftecspath = @"\\ftecs.com\data\";


            if (UseFtecsArchive == true && Directory.Exists(ftecspath))
                DataPath = ftecspath;

        }

        public void ResolveHapiXmlPath()
        {

        }
    }
}