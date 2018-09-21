using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class CDFTesterTimer
    {
        public void Run()
        {
            //WebRequest wr = new WebRequest();
            //wr.Run();

            //string path1 = @"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH\2013\rbsp-a-rbspice_lev-3-PAP_TOFxEH_20130126_v1.1.2-00.cdf";
            //string path2 = @"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH\2013\rbsp-a-rbspice_lev-3-PAP_TOFxEH_20130127_v1.1.2-00.cdf";

            ////CDFReader cdf1 = new CDFReader(path1);
            ////cdf1.Close();

            //CDFReader cdf2 = new CDFReader(path2);
            //cdf2.Close();
            long gavg = 0;
            long davg = 0;
            long favg = 0;
            for (int i = 0; i < 10; i++)
            {
                string[] arg = new string[]
                {
                    //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\",
                    //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\2013",
                    //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH\2013",
                    @"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH\2013",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH",
                    //@"C:\HapiApi",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_0\Auxil\2012",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_0",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_1",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_2",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP",
                };

                CDFTester cdfT = new CDFTester();
                Stopwatch sw = Stopwatch.StartNew();
                cdfT.Run(arg);
                sw.Stop();
                long get = sw.ElapsedMilliseconds;
                gavg += get;

                Console.WriteLine(arg[0]);

                TimeSpan t = TimeSpan.FromMilliseconds(get);
                string finishTime = String.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                            t.Hours,
                                            t.Minutes,
                                            t.Seconds,
                                            t.Milliseconds);
                Console.WriteLine("Total Gazelle execution time: " + finishTime);
                Console.WriteLine();

                arg = new string[]
                {
                    @"D:\2013",
                    //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\",
                    //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\2013",
                    //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH\2013",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH\2013",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH",
                    //@"C:\HapiApi",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_0\Auxil\2012",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_0",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_1",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_2",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP",
                };
                cdfT = new CDFTester();
                sw = Stopwatch.StartNew();
                cdfT.Run(arg);
                sw.Stop();
                long gdet = sw.ElapsedMilliseconds;
                davg += gdet;
                Console.WriteLine(arg[0]);


                t = TimeSpan.FromMilliseconds(gdet);
                finishTime = String.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                            t.Hours,
                                            t.Minutes,
                                            t.Seconds,
                                            t.Milliseconds);
                Console.WriteLine("Total Gazelle D:/ execution time: " + finishTime);
                Console.WriteLine();

                arg = new string[]
                {
                    //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\",
                    //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\2013",
                    @"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH\2013",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH\2013",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH",
                    //@"C:\HapiApi",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_0\Auxil\2012",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_0",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_1",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_2",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3",
                    //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP",
                };
                cdfT = new CDFTester();
                sw = Stopwatch.StartNew();
                cdfT.Run(arg);
                sw.Stop();
                long fet = sw.ElapsedMilliseconds;
                favg += fet;
                Console.WriteLine(arg[0]);

                t = TimeSpan.FromMilliseconds(fet);
                finishTime = String.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                            t.Hours,
                                            t.Minutes,
                                            t.Seconds,
                                            t.Milliseconds);
                Console.WriteLine("Total \\ftecs.com execution time: " + finishTime);
                Console.WriteLine();
            }


            // Average all the times
            double avg = gavg / 10;
            TimeSpan avgt = TimeSpan.FromMilliseconds(avg);
            string avgfinishTime = String.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                            avgt.Hours,
                            avgt.Minutes,
                            avgt.Seconds,
                            avgt.Milliseconds);
            Console.WriteLine("Avg. Gazelle execution time: " + avgfinishTime);
            Console.WriteLine();

            avg = davg / 10;
            avgt = TimeSpan.FromMilliseconds(avg);
            avgfinishTime = String.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                            avgt.Hours,
                            avgt.Minutes,
                            avgt.Seconds,
                            avgt.Milliseconds);
            Console.WriteLine("Avg. Gazelle D:/ execution time: " + avgfinishTime);
            Console.WriteLine();

            avg = favg / 10;
            avgt = TimeSpan.FromMilliseconds(avg);
            avgfinishTime = String.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                            avgt.Hours,
                            avgt.Minutes,
                            avgt.Seconds,
                            avgt.Milliseconds);
            Console.WriteLine("Avg. \\ftecs.com execution time: " + avgfinishTime);
            Console.WriteLine();

            Console.ReadKey();

        }
    }
}
