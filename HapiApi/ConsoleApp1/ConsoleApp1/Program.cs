using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ConsoleApp1;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SPDF.CDF.CSharp;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1
{
    public class Program
    {
        static HttpClient client = new HttpClient();

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://localhost:50112/api/hapi/");
            var parameters = new Dictionary<string, string>
            {
                { "id", "rbspa_rbspice_tofxeh" }
            };

            var encodedContent = new FormUrlEncodedContent(parameters);


            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync("info?id=rbspa_rbspice_tofxeh");
            
            //Console.WriteLine(response.Content.ToString());

            string result = await response.Content.ReadAsStringAsync();
            JObject catalog = JObject.Parse(result);
            //IEnumerable<JToken> list = catalog["catalog"].ToList();
            //foreach (var lis in list)
            //    Console.WriteLine(lis.ToString());

            Console.WriteLine(catalog.ToString());
        }

        public static void Main()
        {
            RunAsync().GetAwaiter().GetResult();
            Console.ReadKey();
        }



    }
}
