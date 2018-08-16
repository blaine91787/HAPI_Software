using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Hapi hapi = new Hapi();
            hapi.CreateCatalog();
            Console.ReadKey();
        } 
    }
}
