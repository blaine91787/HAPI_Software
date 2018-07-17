using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    interface IMover
    {
        int DistanceMoved { get; set; }
        int NumOfStepsMoved { get; set; }
        string Direction { get; set; }
        int Speed { get; }
        void Move(int numsteps, string dir);
    }
}
