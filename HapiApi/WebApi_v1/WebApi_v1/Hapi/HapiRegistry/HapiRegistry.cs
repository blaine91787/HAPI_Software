using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using win32 = Microsoft.Win32;

namespace WebApi_v1.HAPI.Registry
{
    public class HapiRegistry
    {
        public Catalog Catalog { get; set; }

        public HapiRegistry()
        {
            Catalog = new Catalog();
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