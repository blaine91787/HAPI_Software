using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CDFTesterProcessCreator
{
    public static class ConsoleWriter
    {
        public static int OrigRow { get; set; }
        public static int OrigCol { get; set; }        

        public static void WriteAtPosition(string str, int x, int y, bool clear = false)
        {
            try
            {
                Console.SetCursorPosition(OrigCol + x, OrigRow + y);

                if (clear)
                    ClearLine();

                Console.Write(str);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }

        public static void ClearLine(int x = 0)
        {
            int currentLineCursor = Console.CursorTop + x;
            Console.SetCursorPosition(0, currentLineCursor);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public static void Working()
        {
            WriteAtPosition("Working ", 0, 0, true);
            Thread.Sleep(1000);
            WriteAtPosition(". ", Console.CursorLeft, 0);
            Thread.Sleep(1000);
            WriteAtPosition(". ", Console.CursorLeft, 0);
            Thread.Sleep(1000);
            WriteAtPosition(". ", Console.CursorLeft, 0);
            Thread.Sleep(1000);
        }

        public static void ProcessExited(ProcessInfo proc, int procCount)
        {
            string tempstr = procCount + " proccesses left. ";
            ClearLine(-1);
            WriteAtPosition(tempstr, 0, 0);
            Console.WriteLine(proc.CDFName + " has exited.\n");
            OrigRow = Console.CursorTop;
        }

        public static void ElapsedTime(string time)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine("Total execution time in seconds: " + time);
        }
    }
}
