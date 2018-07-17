using CsvHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using WebApi_v1.DataProducts.Utilities;

namespace WebApi_v1.DataProducts.RBSpiceA
{
    public class RBSpiceAProduct : IProduct, IConfigurable
    {
        //new private DataConfiguration Configuration { get; set; }
        public string[] Header { get; set; }
        public List<DataRecord> Records { get; set; }

        public RBSpiceAProduct()
        {
            if (Hapi.Properties == null)
                throw new ArgumentNullException("Hapi Properties must be set before attempting to create RBSpice Product.");
        }

        public bool Configure()
        {
            return true;
        }

        public void GetProduct()
        {
            string basepath = @"C:\Users\blaine.harris\Documents\BlainesProjects\WebApi_v1\WebApi_v1\obj\Debug";
            string path = @"\rbsp-a-rbspice_lev-0_Auxil_20121028_v1.1.1-00.csv";
            Type recordtype;
            if ("Auxiliary" == nameof(Auxiliary))
                recordtype = typeof(Auxiliary);
            Records = new List<DataRecord>();
            if (File.Exists(path))
            {
                using (FileStream fs = File.OpenRead(path))
                using (TextReader textReader = new StreamReader(fs))
                {

                    CsvReader csv = new CsvReader(textReader);
                    csv.Configuration.RegisterClassMap<AuxiliaryMap>();
                    csv.Read();
                    csv.ReadHeader();
                    this.Header = csv.Context.HeaderRecord;
                    this.Records = csv.GetRecords<Auxiliary>().ToList<DataRecord>();
                }
            }
            else
            {
                Debug.WriteLine("FAAAAAAIIIIIIILLLLLL");
            }
        }

        public void GetProductWithTimeRange()
        {
            string path = @"C:\Users\blaine.harris\Documents\BlainesProjects\WebApi_v1\WebApi_v1\obj\Debug\rbsp-a-rbspice_lev-0_Auxil_20121028_v1.1.1-00.csv";
            Type recordtype;
            if ("Auxiliary" == nameof(Auxiliary))
                recordtype = typeof(Auxiliary);
            Records = new List<DataRecord>();
            if (File.Exists(path))
            {
                using (FileStream fs = File.OpenRead(path))
                using (TextReader textReader = new StreamReader(fs))
                {

                    CsvReader csv = new CsvReader(textReader);
                    csv.Configuration.RegisterClassMap<AuxiliaryMap>();
                    csv.Read();
                    csv.ReadHeader();
                    this.Header = csv.Context.HeaderRecord;
                    //this.Records = csv.GetRecords<T>().ToList<T>();
                    while (csv.Read())
                    {
                        if (Converters.ConvertUTCtoDate(csv["UTC"]) >= TimeRange.Start
                            && Converters.ConvertUTCtoDate(csv["UTC"]) <= TimeRange.End)
                        {
                            if ("Auxiliary" == nameof(Auxiliary))
                                Records.Add(csv.GetRecord<Auxiliary>());
                        }
                    }
                };
            }
            else
            {
                Debug.WriteLine("FAAAAAAIIIIIIILLLLLL");
            }


        }
    }
}