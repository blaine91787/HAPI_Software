﻿using CDF;
using SPDF.CDF.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using WebApi_v1.HAPI.Registry;
using WebApi_v1.HAPI.Utilities;

namespace WebApi_v1.HAPI.Catalog
{
    public class CatalogProducer
    {        //private string _xmlCatalogPath = @"testXmlCatalog.xml";
        private string _xmlCatalogPath = @"C:\Users\blaine.harris\Documents\Github\FTECS\HapiApi\WebApi_v1\WebApi_v1\Hapi\HapiXml\HapiCatalog.xml";
        private string _productPath = @"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\"; //\TOFxEH\2013\rbsp-a-rbspice_lev-3-PAP_TOFxEH_20130126_v1.1.2-00.cdf";
        private bool _clearProducts = true; // Only for testing. Set false otherwise. (Keeps the auxiliary product available and refreshes the level_3PAP products.

        public void CreateCatalog()
        {

            // Check if the catalog has been updated today, if so then return because it's already up to date.;

            HapiRegistry hr = new HapiRegistry();
            DateTime currentDateTime = DateTime.Now.Date;
            DateTime lastUpdate = default(DateTime);
            
            DateTime.TryParse(hr.Catalog.LastUpdate, out lastUpdate);

            if (!(lastUpdate < currentDateTime))
                return;



            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(_xmlCatalogPath);


            // Update the catalog's lastupdated date and populate with the new information
            XmlNodeList instruments = xdoc.GetElementsByTagName("instrument");
            XmlNode instrument = instruments[instruments.Count - 1];

            if (_clearProducts)
                ClearProducts(xdoc); // TODO: Once the auxiliary product is removed, update this to remove products and repopulate (or just update the dates to save on performance)

            if (!Directory.Exists(_productPath))
                throw new DirectoryNotFoundException();

            string[] paths = Directory.GetDirectories(_productPath);
            List<string> filetypes = new List<string>();
            List<FileInfo> listoffiles = new List<FileInfo>();
            Queue<TimeRangeFiles> timeFiles = new Queue<TimeRangeFiles>();
            foreach (string path in paths)
            {
                string yearpath = Directory.GetDirectories(path).FirstOrDefault();

                if (path == paths.First())
                {
                    if (yearpath != default(string) && Directory.GetFiles(yearpath, "*.cdf").Length != 0)
                        filetypes.Add("cdf");
                    if (yearpath != default(string) && Directory.GetFiles(yearpath, "*.csv.gz").Length != 0)
                        filetypes.Add("csv.gz");
                    if (yearpath != default(string) && Directory.GetFiles(yearpath, "*.csv").Length != 0)
                        filetypes.Add("csv");
                }

                TimeRangeFiles trf = new TimeRangeFiles();
                string firstYearDirectory = Directory.GetDirectories(path).FirstOrDefault();
                string lastYearDirectory = Directory.GetDirectories(path).LastOrDefault();
                FileInfo startFI = default(FileInfo);
                FileInfo stopFI = default(FileInfo);
                GetStartFile(firstYearDirectory, ref startFI, out _);
                GetStopFile(lastYearDirectory, ref stopFI, out _);
                trf.StartFile = startFI;
                trf.StopFile = stopFI;
                timeFiles.Enqueue(trf);


                FileInfo fi = new FileInfo(Directory.GetFiles(yearpath).FirstOrDefault());
                listoffiles.Add(fi);
            }

            List<XmlElement> products = GetProducts(timeFiles, xdoc, listoffiles, filetypes.FirstOrDefault());

            foreach (XmlElement product in products)
                instrument.AppendChild(product);

            xdoc.Save(_xmlCatalogPath);
            hr.Catalog.SetLastUpdate(currentDateTime);
        }

        private void GetStartFile(string path, ref FileInfo fi, out CDFReader cdf)
        {
            IEnumerable<string> filePaths = Directory.GetFiles(path);
            CDFReader tempcdf = default(CDFReader);
            foreach (string filePath in filePaths)
            {
                fi = new FileInfo(filePath);
                try { tempcdf = new CDFReader(fi.FullName); cdf = tempcdf; return; }
                catch (Exception exc) { Debug.WriteLine("{0}", exc.Message); continue; } // TODO: Handle Exception
            }
            throw new Exception("Critical Error: Start file was not found. ");
        }

        private void GetStopFile(string path, ref FileInfo fi, out CDFReader cdf)
        {
            IEnumerable<string> filePaths = Directory.GetFiles(path).Reverse();
            CDFReader tempcdf = default(CDFReader);
            foreach (string filePath in filePaths)
            {
                fi = new FileInfo(filePath);
                try { tempcdf = new CDFReader(fi.FullName); cdf = tempcdf; return; }
                catch (Exception exc) { Debug.WriteLine("{0}", exc.Message); } // TODO: Handle Exception
            }
            throw new Exception("Critical Error: Stop file was not found. ");
        }

        private List<XmlElement> GetProducts(Queue<TimeRangeFiles> trfQueue, XmlDocument xdoc, List<FileInfo> listoffiles, string filetype)
        {
            List<XmlElement> list = new List<XmlElement>();
            foreach (FileInfo tempFile in listoffiles)
            {
                FileInfo fi = default(FileInfo);
                CDFReader cdf = default(CDFReader);

                try { GetStartFile(tempFile.Directory.FullName, ref fi, out cdf); }
                catch (Exception exc) { Debug.WriteLine("{0}", exc.Message); } // TODO: Handle Exception


                string[] splitPath = fi.DirectoryName.Split('\\');
                string level = splitPath[9];
                string name = splitPath[10];
                string sc = splitPath[6].ToLower();
                string instr = splitPath[7].ToLower();
                string id = name.ToLower();
                string hapiId = String.Format("{0}_{1}_{2}", sc, instr, id);
                string prodpath = String.Format(@"$data$\{0}\{1}\", level, name);
                string title = String.Empty;
                string starttime = String.Empty;
                string stoptime = String.Empty;
                try { title = cdf.Doc.FindAttribute("LINK_TEXT").GetValue(0, -1).Value.ToString(); } catch { }
                TimeRangeFiles trf = trfQueue.Dequeue();
                trf.GetTimeRange(out starttime, out stoptime);

                if (ProductExists(xdoc, hapiId))
                    continue;

                XmlElement product = xdoc.CreateElement("product");
                product.SetAttribute("name", name);
                product.SetAttribute("id", name.ToLower());
                product.SetAttribute("hapiId", hapiId);
                product.SetAttribute("filetype", filetype);
                product.SetAttribute("starttime", starttime);
                product.SetAttribute("stoptime", stoptime);
                product.SetAttribute("title", title);
                product.SetAttribute("path", prodpath);

                List<XmlElement> fields = GetFields(xdoc, cdf.Doc);

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

        private void ClearProducts(XmlDocument xdoc)
        {
            XmlNodeList products = xdoc.GetElementsByTagName("product");
            XmlNode prod = products.Item(products.Count - 1);
            if (products.Count > 1)
            {
                prod.ParentNode.RemoveChild((XmlNode)prod);
                ClearProducts(xdoc);
            }
        }

        private List<XmlElement> GetFields(XmlDocument xdoc, CDF_File cdf)
        {

            List<XmlElement> list = new List<XmlElement>();

            foreach (CDF_Variable var in cdf.Variables)
            {
                //Debug.WriteLine(String.Format("{0}", var.Name));
                XmlElement field = xdoc.CreateElement("field");

                cdf.HandleAllExceptions = true;

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

        private class TimeRangeFiles
        {
            public FileInfo StartFile { get; set; }
            public FileInfo StopFile { get; set; }

            public string GetStartTime()
            {
                string startTime = String.Empty;
                IEnumerable<string> files = Directory.GetFiles(StartFile.DirectoryName);
                foreach (string file in files)
                {
                    CDFReader cdf = default(CDFReader);
                    try
                    {
                        cdf = new CDFReader(file);
                        CDF_Variable utcVar = cdf.GetVariable("UTC");
                        startTime = utcVar[0].ToString().Trim();
                    }
                    catch (Exception exc) { Debug.WriteLine(exc.Message); continue; }

                    break;
                }
                return startTime;
            }

            public string GetStopTime()
            {
                string stopTime = String.Empty;
                IEnumerable<string> files = Directory.GetFiles(StopFile.DirectoryName).Reverse();
                foreach (string file in files)
                {
                    CDFReader cdf = default(CDFReader);
                    CDF_Variable utcVar = default(CDF_Variable);
                    int utcVarLastIndex = -1;
                    try
                    {
                        cdf = new CDFReader(file);
                        utcVar = cdf.GetVariable("UTC");
                        utcVarLastIndex = utcVar.Records - 1;
                        stopTime = utcVar[utcVarLastIndex].ToString().Trim();
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine(exc.Message);
                        continue;
                    }

                    break;
                }

                return stopTime;
            }

            public void GetTimeRange(out string starttime, out string stoptime)
            {
                starttime = stoptime = String.Empty;
                starttime = GetStartTime();
                stoptime = GetStopTime();
            }
        }
    }
}