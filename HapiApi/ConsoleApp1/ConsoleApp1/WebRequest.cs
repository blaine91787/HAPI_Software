using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class WebRequest
    {

        static HttpClient client = new HttpClient();

        public static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://localhost:50112/api/hapi/");


            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            //HttpResponseMessage response = await client.GetAsync("info?id=rbspa_rbspice_tofxeh");
            HttpResponseMessage response = await client.GetAsync("data?id=rbspa_rbspice_tofxeh&time.min=2013-01-26T&time.max=2013-01-26T10:20Z&format=csv");
            string result = await response.Content.ReadAsStringAsync();


            //JObject catalog = JObject.Parse(result);
            //foreach (var str in catalog["data"].ToList())
            //{
            //    Console.WriteLine(str);
            //}

            //Console.WriteLine(catalog.ToString());
            bool? x;
            if (response.IsSuccessStatusCode)
            {
                x = await CsvParser.Parse(response);
            }
            else
            {
                Console.WriteLine("Request failed.");
                Console.WriteLine(response.Content.ReadAsStringAsync());
            }

        }

        public void Run()
        {

            RunAsync().GetAwaiter().GetResult();
        }
    }
}
