using SPDF.CDF.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleApp1
{
    public class XmlCatalogProducer
    {
        //private string _xmlCatalogPath = @"testXmlCatalog.xml";
        private string _xmlCatalogPath = @"C:\Users\blaine.harris\Documents\Github\FTECS\HapiApi\WebApi_v1\WebApi_v1\Hapi\HapiXml\HapiCatalog.xml";
        private string _productPath = @"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\"; //\TOFxEH\2013\rbsp-a-rbspice_lev-3-PAP_TOFxEH_20130126_v1.1.2-00.cdf";


        public void CreateCatalog()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(_xmlCatalogPath);
            XmlNodeList instruments = xdoc.GetElementsByTagName("instrument");
            XmlNode instrument = instruments[instruments.Count - 1];

            if (!Directory.Exists(_productPath))
                throw new DirectoryNotFoundException();

            string[] paths = Directory.GetDirectories(_productPath);

            List<FileInfo> listoffiles = new List<FileInfo>();
            foreach (string path in paths)
            {
                string yearpath = Directory.GetDirectories(path).FirstOrDefault();
                FileInfo fi = new FileInfo(Directory.GetFiles(yearpath).FirstOrDefault());
                listoffiles.Add(fi);
            }

            List<XmlElement> products = GetProducts(xdoc, listoffiles);

            foreach (XmlElement product in products)
                instrument.AppendChild(product);

            xdoc.Save(_xmlCatalogPath);
        }

        private List<XmlElement> GetProducts(XmlDocument xdoc, List<FileInfo> listoffiles)
        {
            List<XmlElement> list = new List<XmlElement>();
            foreach (FileInfo fi in listoffiles)
            {

                CDFReader cdf = new CDFReader(fi.FullName);
                CDF_File cdfFi = new CDF_File(fi.FullName);


                string[] splitPath = fi.DirectoryName.Split('\\');
                string level = splitPath[9];
                string name = splitPath[10];
                string sc = splitPath[6].ToLower();
                string instr = splitPath[7].ToLower();
                string id = name.ToLower();
                string hapiId = String.Format("{0}_{1}_{2}", sc, instr, id);
                string prodpath = String.Format(@"$data$\{0}\{1}\", level, name);
                string title = String.Empty;
                try { title = cdfFi.FindAttribute("LINK_TEXT").GetValue(0, -1).Value.ToString(); } catch { }

                if (ProductExists(xdoc, hapiId))
                    continue;

                XmlElement product = xdoc.CreateElement("product");
                product.SetAttribute("name", name);
                product.SetAttribute("id", name.ToLower());
                product.SetAttribute("hapiId", hapiId);
                product.SetAttribute("title", title);
                product.SetAttribute("path", prodpath);

                List<XmlElement> fields = GetFields(xdoc, cdfFi);

                foreach (XmlElement field in fields)
                    product.AppendChild(field);

                list.Add(product);
            }
            return list;
        }
        private bool ProductExists(XmlDocument xdoc, string hapiId)
        {
            XmlNodeList products = xdoc.GetElementsByTagName("product");

            foreach (XmlElement product in products)
                if (product.GetAttribute("hapiId").Equals(hapiId))
                    return true;

            return false;
        }
        private List<XmlElement> GetFields(XmlDocument xdoc,CDF_File cdf)
        {

            List<XmlElement> list = new List<XmlElement>();

            foreach (CDF_Variable var in cdf.Variables)
            {
                Debug.WriteLine(String.Format("{0}", var.Name));
                XmlElement field = xdoc.CreateElement("field");

                cdf.HandleAllExceptions = false;

                string name, type, units, fill, length, description, bins, size;
                name = type = units = fill = length = description = bins = size = String.Empty;

                try { name = var.Name; } catch { }
                try { type = var.WindowsType.Name; } catch { }
                try { units = var.GetAttribute("UNITS").GetValue(0, var).Value.ToString(); } catch { }
                try { fill = var.GetAttribute("FILLVAL").GetValue(0, var).Value.ToString(); } catch { }
                try { length = var.GetAttribute("LENGTH").GetValue(0, var).Value.ToString(); } catch { }
                try { description = var.GetAttribute("CATDESC").GetValue(0, var).Value.ToString(); } catch { }
                try { bins = var.GetAttribute("BINS").GetValue(0, var).Value.ToString(); } catch { }
                try { size = var.GetAttribute("SIZE").GetValue(0, var).Value.ToString(); } catch { }


                field.SetAttribute("name", name == String.Empty ? "null" : name);
                field.SetAttribute("type", type == String.Empty ? "null" : type);
                field.SetAttribute("units", units == String.Empty ? "null" : units);
                field.SetAttribute("fill", fill == String.Empty ? "null" : fill);
                field.SetAttribute("length", length == String.Empty ? "null" : length);
                field.SetAttribute("description", description == String.Empty ? "null" : description);
                field.SetAttribute("bins", bins == String.Empty ? "null" : bins);
                field.SetAttribute("size", size == String.Empty ? "null" : size);

                list.Add(field);
            }

            return list;
        }
    }
}
