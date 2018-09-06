using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using win32 = Microsoft.Win32;

namespace WebApi_v1.HAPI.Registry
{
    public class HapiRegistry
    {
        public string Version { get; set; }
        public string DataPath { get; set; }
        public string Level3PAPPath { get; set; }
        public string TestDataPath { get; set; }
        public string UserPath { get; set; }
        public string UseFtecsData { get; set; }
        public Catalog Catalog { get; set; }

        public HapiRegistry()
        {
            String key = @"Software\FundamentalTechnologies\NASA\HAPI\";
            win32.RegistryKey rkHKLM = win32.Registry.LocalMachine;
            win32.RegistryKey rkHAPI;

            try
            {
                rkHAPI = rkHKLM.OpenSubKey(key);
            }
            catch
            {
                throw new Exception("The HAPI registry key does not exist");
            }

            GetKeys(rkHAPI);
            Catalog = new Catalog();
        }

        public void GetKeys(win32.RegistryKey rkHAPI)
        {
            try { Version = (string)rkHAPI.GetValue("Version"); } catch { throw new Exception("Error getting version key"); }
            try { DataPath = (string)rkHAPI.GetValue("DataPath"); } catch { throw new Exception("Error getting datapath key"); }
            try { Level3PAPPath = (string)rkHAPI.GetValue("Level3PAPPath"); } catch { throw new Exception("Error getting level3pappath key"); }
            try { TestDataPath = (string)rkHAPI.GetValue("TestDataPath"); } catch { throw new Exception("Error getting testdatapath key"); }
            try { UserPath = (string)rkHAPI.GetValue("UserPath"); } catch { throw new Exception("Error getting userpath key"); }
            try { UseFtecsData = (string)rkHAPI.GetValue("UseFtecsData"); } catch { throw new Exception("Error getting UseFtecsData key"); }
        }
    }


    public class Catalog
    {
        public String LastUpdate { get; private set; }

        public Catalog()
        {
            GetLastUpdate();
        }

        public void GetLastUpdate()
        {
            String key = @"Software\FundamentalTechnologies\NASA\HAPI\Catalog";

            win32.RegistryKey rkHKLM = win32.Registry.LocalMachine;
            win32.RegistryKey rkHAPI;
            try
            {
                // load the definitions for the overall mission information
                rkHAPI = rkHKLM.OpenSubKey(key);
                LastUpdate = (String)rkHAPI.GetValue("LastUpdate");
            }
            catch
            {
                throw new Exception("Registry Key = " + key + " does not exist");
            }
        }

        public void SetLastUpdate(DateTime currentUTC)
        {
            
            String key = @"Software\FundamentalTechnologies\NASA\HAPI\Catalog";

            win32.RegistryKey rkHKLM = win32.Registry.LocalMachine;
            win32.RegistryKey rkHAPI;
            try
            {
                // load the definitions for the overall mission information
                rkHAPI = rkHKLM.OpenSubKey(key, true);
                rkHAPI.SetValue("LastUpdate", currentUTC.ToString("yyyy-MM-dd"));
            }
            catch
            {
                throw new Exception("Registry Key = " + key + " does not exist");
            }
        }
    }
}