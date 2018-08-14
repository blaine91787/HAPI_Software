using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApi_v1.Hapi
{
    public static class HapiPaths
    {
        public static string UserPath = String.Empty;
        public static string SoftwarePath = String.Empty;
        public static string DataPath = String.Empty;

        public static void Resolve()
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
                throw new DirectoryNotFoundException("RBSPiceAProduct._basepath could not resolve to a valid path.");


            SoftwarePath = UserPath + @"\Documents\Github\FTECS\HapiApi\WebApi_v1\WebApi_v1\";
            DataPath = SoftwarePath + @"\SCRecords\";
        }
    }
}