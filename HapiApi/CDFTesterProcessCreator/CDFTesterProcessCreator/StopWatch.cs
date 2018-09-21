using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDFTesterProcessCreator
{
    public class SWatch
    {
        public long MS { get; set; }
        public TimeSpan TS { get; set; }
        public string Average { get; set; }

        public SWatch()
        {
            MS = 0;
            TS = default(TimeSpan);
        }

        public void PrintTime(bool showErrors = false)
        {
            try
            {
                TS = TimeSpan.FromMilliseconds(MS);
                string str = String.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                TS.Hours,
                                TS.Minutes,
                                TS.Seconds,
                                TS.Milliseconds);
                ConsoleWriter.ElapsedTime(str);
            }
            catch (Exception e)
            {
                if(showErrors)
                {
                    ConsoleWriter.WriteAtPosition(e.Message, 0, 0);
                }
            }
        }

        public void CalculateAverage(int numProcsExited, int numProcsRunning)
        {
            MS = MS / numProcsExited;
            MS = MS * numProcsRunning;

            TS = TimeSpan.FromMilliseconds(MS);
            Average = String.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                        TS.Hours,
                                        TS.Minutes,
                                        TS.Seconds,
                                        TS.Milliseconds);

        }


    }
}
