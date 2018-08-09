using System;

namespace WebApi_v1.HapiUtilities
{
    public class TimeRange
    {
        public bool InTimeRange { get; set; }
        public bool STime { get; set; }
        public bool ETime { get; set; }
        public DateTime UserMin { get; set; }
        public DateTime UserMax { get; set; }
        public DateTime Min { get; set; }
        public DateTime Max { get; set; }

        public TimeRange()
        {
            InTimeRange = false;
            UserMin = default(DateTime);
            UserMax = default(DateTime);
            Min = default(DateTime);
            Max = default(DateTime);
        }

        public bool IsValid()
        {
            // TODO: Find what the spec expects for min and max times
            if (UserMin.Date >= Min.Date && UserMax.Date <= Max.Date)
            {
                InTimeRange = true;
                return true;
            }
            return false;
        }
    }
}