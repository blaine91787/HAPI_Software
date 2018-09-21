using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDFTesterProcessCreator
{
    public class ProcessInfo
    {
        public Process Process { get; set; }
        public string CDFName { get; set; }
        public Stopwatch Clock { get; set; }
    }
}
