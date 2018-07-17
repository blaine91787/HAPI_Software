using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string query1 = "?id=RBSpiceA";
            string query2 = "?id=RBSpiceA&time.min=2012-10-28T12:01Z";
            string query3 = "?id=RBSpiceA&time.min=2012-10-28T12:01Z&time.max=2012-10-28T12:05Z";
            string query4 = "?id=RBSpiceA&time.min=2012-10-28T12:01Z&time.max=2012-10-28T12:05Z&parameters=a,b,c,d,e,f,g,h,i,j&lksjdflkj=lksjdflkjsdf";

            char[] delimiters = { '?', '&' , '='};
            query4 = query4.ToLower().TrimStart(delimiters);
            string[] x = query4.Split(delimiters);
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            for ( int i = 0; i<x.Length; i+=2)
                parameters.Add(x[i], x[i + 1]);
            
            foreach (var str in parameters)
                Console.WriteLine(str);

            try
            {
                RequestParameters.Assign(parameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine(RequestParameters.Output());

            Console.ReadKey();
        }

        public static class RequestParameters
        {
            private static char[] _delimiters = { '?', '&', '.', ',' };
            public static string Id { get; set; }
            public static DateTime TimeMin { get; set; }
            public static DateTime TimeMax { get; set; }
            public static List<string> Parameters { get; set; } = null;
            public static bool IncludeHeader { get; set; } = false;

            public static void Assign(Dictionary<string, string> dict)
            {
                string key = String.Empty;
                string val = String.Empty;
                DateTime dt = default(DateTime);
                foreach ( KeyValuePair<string,string> pair in dict )
                {
                    key = pair.Key.ToLower();
                    val = pair.Value.ToLower();
                    switch (key)
                    {
                        case ("id"):
                            Id = val;
                            break;
                        case ("time.min"):
                            dt = Convert.ToDateTime(val);
                            if(dt != null)
                            TimeMin = dt;
                            break;
                        case ("time.max"):
                            dt = Convert.ToDateTime(val);
                            if (dt != null)
                                TimeMax = dt;
                            break;
                        case ("parameters"):
                            Parameters = new List<string>();
                            string[] pars = val.Split(new char[] { ',' });
                            Parameters = pars.ToList();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            public static string Output()
            {
                string str = "";
                string pars = String.Empty;
                foreach (var par in RequestParameters.Parameters)
                {
                    pars += par.ToString() + ", ";
                }
                str += String.Format("ID: {0}\nTimeMin: {1}\nTimeMax: {2}\nParameters: {3}\nIncludeHeader: {4}\n",
                                        RequestParameters.Id,
                                        RequestParameters.TimeMin,
                                        RequestParameters.TimeMax,
                                        pars,
                                        RequestParameters.IncludeHeader);

                return str;
            }
        }

        public static class TimeRange
        {
            public static bool IsTimeRange = false;
            public static bool STime { get; set; }
            public static bool ETime { get; set; }
            public static DateTime Start { get; set; }
            public static DateTime End { get; set; }

            public static void Reset()
            {
                IsTimeRange = false;
                STime = false;
                ETime = false;
                Start = default(DateTime);
                End = default(DateTime);
            }
        }
    }
}
