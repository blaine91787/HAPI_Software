﻿using System.Collections.Generic;
using System.IO;
using WebApi_v1.Hapi;

namespace WebApi_v1.HapiDataProducts
{
    public abstract class Product
    {
        public HapiConfiguration HapiConfig { get; set; }
        public List<string> Paths { get; set; }
        public List<FileInfo> Files { get; set; }

        public string[] Header { get; set; }
        public IEnumerable<Dictionary<string, string>> Records { get; set; }

        public abstract void Configure(HapiConfiguration hapi);
        public abstract bool GetProduct();
        public abstract bool VerifyTimeRange();
        public abstract void GetPaths();
    }
}