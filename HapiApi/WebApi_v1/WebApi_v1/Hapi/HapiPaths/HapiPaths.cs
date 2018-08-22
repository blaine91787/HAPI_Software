using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApi_v1.HAPI
{
    public class HapiPaths
    {
        private bool UseFtecsArchive = false;
        private bool UseGazelleArchive = false;
        public string UserPath = String.Empty;
        public string SoftwarePath = String.Empty;
        public string DataPath = String.Empty;

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


            SoftwarePath = UserPath + @"\Documents\Github\FTECS\HapiApi\WebApi_v1\WebApi_v1\";
            DataPath = SoftwarePath + @"\SCRecords\";
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