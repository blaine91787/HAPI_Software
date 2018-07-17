using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_v1.DataProducts.Utilities
{
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