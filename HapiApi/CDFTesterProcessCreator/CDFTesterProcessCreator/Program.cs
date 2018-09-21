using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CDFTesterProcessCreator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string[] arg = new string[]
            {
                @"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH\2013",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\ESRHELT",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH",
                //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\ESRHELT",
                //@"C:\HapiApi\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH\2013",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH\2014",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH\2015",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH\2016",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH\2017",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEH\2018",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\ESRHELT\2012",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\ESRHELT\2013",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\ESRHELT\2014",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\ESRHELT\2015",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\ESRHELT\2016",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\ESRHELT\2017",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\ESRHELT\2018",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEHe\2013",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEHe\2014",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEHe\2015",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEHe\2016",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEHe\2017",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEHe\2018",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEIon\2012",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEIon\2013",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEIon\2014",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEIon\2015",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEIon\2016",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEIon\2017",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEIon\2018",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEO\2013",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEO\2014",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEO\2015",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEO\2016",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEO\2017",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEO\2018",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxPHHHELT\2012",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxPHHHELT\2013",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxPHHHELT\2014",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxPHHHELT\2015",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxPHHHELT\2016",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxPHHHELT\2017",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxPHHHELT\2018",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxPHOHELT\2012",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxPHOHELT\2013",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxPHOHELT\2014",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxPHOHELT\2015",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxPHOHELT\2016",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxPHOHELT\2017",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxPHOHELT\2018",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP\TOFxEHe",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP",
                //@"\\ftecs.com\data\Archive\RBSP\RBSPA\RBSPICE\Data\Level_3PAP",
            };
            if (args.Length > 0)
            {
                if (Directory.Exists(args[0]))
                {
                    IEnumerable<string> paths = PathRetriever.GetPaths(args);

                    List<ProcessInfo> procs = new List<ProcessInfo>();

                    int count = 0;
                    var watch = Stopwatch.StartNew();
                    long elapsedMs = 0;
                    foreach (string path in paths)
                    {
                        var psi = new ProcessStartInfo
                        {
                            FileName = @"C:\Users\blaine.harris\Documents\Github\FTECS\HapiApi\ConsoleApp1\ConsoleApp1\bin\x64\Release\ConsoleApp1.exe",
                            Arguments = path,
                            UseShellExecute = false
                        };
                        var p = Process.Start(psi);
                        string[] pathsplit = path.Split('\\');
                        string product = pathsplit[pathsplit.Length - 2];
                        string year = pathsplit[pathsplit.Length - 1];
                        ProcessInfo pInfo = new ProcessInfo()
                        {
                            Process = p,
                            CDFName = product + year,
                            Clock = Stopwatch.StartNew()
                        };
                        procs.Add(pInfo);
                        //Console.Write("Starting process # " + pInfo.Process.Id + " for : ");
                        //Console.WriteLine(pInfo.CDFName + "\n");
                    }

                    int numProcsStarted = procs.Count();
                    List<Stopwatch> watches = new List<Stopwatch>();
                    long estTime = 0;
                    TimeSpan timeTilFinished;
                    string finishTime = "";
                    int hrs = 0;
                    int mins = 0;
                    int secs = 0;
                    int ms = 0;


                    Console.WriteLine(numProcsStarted + " proccesses have been initiated.\n");
                    ConsoleWriter.OrigRow = Console.CursorTop;
                    ConsoleWriter.OrigCol = Console.CursorLeft;
                    SWatch elTime = new SWatch();
                    SWatch esTime = new SWatch();
                    while (procs.Count() > 0)
                    {

                        ConsoleWriter.Working();
                        
                        foreach (ProcessInfo proc in procs)
                        {
                            if (proc.Process.HasExited)
                            {
                                proc.Clock.Stop();
                                watches.Add(proc.Clock);
                                procs.Remove(proc);
                                ConsoleWriter.ProcessExited(proc, procs.Count());
                                break;
                            }
                        }

                       
                        foreach (Stopwatch sw in watches)
                        {
                            esTime.MS += sw.ElapsedMilliseconds;
                        }

                        if (watches.Count() > 0)
                        {
                            esTime.CalculateAverage(numProcsStarted, procs.Count());
                        }

                    }
                    watch.Stop();
                    elTime.MS = watch.ElapsedMilliseconds;              
                    elTime.PrintTime();
                }
            }
            Console.WriteLine("CDFTester has finished...\n");
        }
    }
}
