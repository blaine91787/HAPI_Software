using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Data
    {
        public Double[] data { get; set; }
    }

    public static class Mappings
    {
        public sealed class Tofxeh_Map : ClassMap<Data>
        {
            public Tofxeh_Map()
            {
                AutoMap();
            }
        }
    }
}
