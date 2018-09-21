using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDFTesterProcessCreator
{
    public static class PathRetriever
    {
        private static List<string> _parentDirectories = new List<string>();

        public static List<string> GetPaths(IEnumerable<string> paths)
        {
            List<string> temp = new List<string>();

            foreach(string path in paths)
            {
                int x = 0;
                if(!Int32.TryParse(path.Split('\\').Last(), out x))
                    _parentDirectories.Add(path);
                RecursivelyGetPaths(path, temp);
            }

            return temp;
        }

        private static void RecursivelyGetPaths(string path, List<string> paths)
        {
            List<string> temp = new List<string>();
            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
                if (dirs.Length != 0)
                    RecursivelyGetPaths(dir, paths);


            if (!_parentDirectories.Contains(path))
            {
                _parentDirectories.Add(Directory.GetParent(path).FullName);
                if (Directory.GetFiles(path, "*.cdf").Count() != 0)
                    paths.Add(path);
            }
        }

    }
}
