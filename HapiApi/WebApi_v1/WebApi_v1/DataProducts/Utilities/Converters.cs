using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace WebApi_v1.DataProducts.Utilities
{
    public static class Converters
    {
        public static DateTime ConvertUTCtoDate(String utc)
        {
            String[] seps = new String[4] { "-", ":", "T", "." };
            String[] parts = utc.Split(seps, StringSplitOptions.None);

            if (parts.Length == 0) return new DateTime();

            Int32 yr = 0;
            Int32 doy = 0;
            Int32 month = 0;
            Int32 day = 0;
            Int32 hour = 0;
            Int32 min = 0;
            Int32 sec = 0;
            Int32 milli = 0;
            if (parts.Length > 0)
            {
                if (!Int32.TryParse(parts[0], out yr)) return new DateTime();

                if (parts.Length > 1)
                {
                    if (!Int32.TryParse(parts[1], out doy)) return new DateTime(yr, DateTimeKind.Utc);
                    ConvertDOYtoMD(yr, doy, out month, out day);

                    if (parts.Length > 2)
                    {
                        if (!Int32.TryParse(parts[2], out hour)) return new DateTime(yr, month, day, 0, 0, 0, DateTimeKind.Utc);

                        if (parts.Length > 3)
                        {
                            if (!Int32.TryParse(parts[3], out min)) return new DateTime(yr, month, day, hour, 0, 0, DateTimeKind.Utc);

                            if (parts.Length > 4)
                            {
                                if (!Int32.TryParse(parts[4], out sec)) return new DateTime(yr, month, day, hour, min, 0, DateTimeKind.Utc);

                                if (parts.Length > 5)
                                {
                                    if (!Int32.TryParse(parts[5], out milli)) return new DateTime(yr, month, day, hour, min, sec, DateTimeKind.Utc);
                                    FixDateValues(ref yr, ref month, ref day, ref hour, ref min, ref sec, ref milli);
                                    return new DateTime(yr, month, day, hour, min, sec, milli, DateTimeKind.Utc);
                                }
                                else return new DateTime(yr, month, day, hour, min, sec, DateTimeKind.Utc);
                            }
                            else return new DateTime(yr, month, day, hour, min, sec, DateTimeKind.Utc);
                        }
                        else return new DateTime(yr, month, day, hour, min, sec, DateTimeKind.Utc);
                    }
                    else return new DateTime(yr, month, day, hour, min, sec, DateTimeKind.Utc);
                }
                else return new DateTime(yr, month, day, hour, min, sec, DateTimeKind.Utc);
            }
            else return new DateTime();
        }

        public static void ConvertDOYtoMD(Int32 year, Int32 doy, out Int32 month, out Int32 day)
        {
            Int32[] nlyDays = new Int32[13] { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
            Int32[] lyDays = new Int32[13] { 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366 };
            month = 0;
            day = 0;

            if ((year % 4) == 0) // leap year
            {
                if (doy < 1) doy = 1;
                if (doy > 366) doy = 366;
                Int32 mnth = 0;
                while (doy > lyDays[mnth]) mnth++;
                month = mnth--;
                day = doy - lyDays[mnth];
            }
            else // not leap year
            {
                if (doy < 1) doy = 1;
                if (doy > 365) doy = 365;
                Int32 mnth = 0;
                while (doy > nlyDays[mnth]) mnth++;
                month = mnth--;
                day = doy - nlyDays[mnth];
            }
            return;
        }

        public static void FixDateValues(ref Int32 yr, ref Int32 month, ref Int32 day, ref Int32 hour, ref Int32 min, ref Int32 sec, ref Int32 mill)
        {
            // purpose of this is to fix date and time values that are anomalous but still numbers and otherwise assumed correct it is just their form isn't copacetic with the Windows DateTime function, e.g. sec=60
            if ((mill < 0) || (mill > 999))
            {
                if (mill < 0) while (mill < 0) { sec--; mill += 1000; }
                else while (mill > 999) { sec++; mill -= 1000; }
            }

            if ((sec < 0) || (sec > 59))
            {
                if (sec < 0) while (sec < 0) { min--; sec += 60; }
                else while (sec > 59) { min++; sec -= 60; }
            }

            if ((min < 0) || (min > 59))
            {
                if (min < 0) while (min < 0) { hour--; min += 60; }
                else while (min > 59) { hour++; min -= 60; }
            }

            if ((hour < 0) || (hour > 23))
            {
                if (hour < 0) while (hour < 0) { day--; hour += 24; }
                else while (hour > 23) { day++; hour -= 24; }
            }

            Int32 lastdoy = EndingDOY(yr);

            Int32 doy = GetDOYfromMD(yr, month, day);

            if ((doy < 0) || (doy > lastdoy))
            {
                if (doy < 0)
                {
                    while (doy < 0)
                    {
                        yr--;
                        doy = EndingDOY(yr) + doy;
                    }
                }
                else
                {
                    while (doy > lastdoy)
                    {
                        yr++;
                        doy = doy - lastdoy;
                        lastdoy = EndingDOY(yr);
                    }
                }
            }

            // now convert the doy value back to a month/day values
            ConvertDOYtoMD(yr, doy, out month, out day);
        }

        public static Int32 EndingDOY(Int32 year)
        {
            if ((year % 4) == 0) return 366;
            return 365;
        }

        public static Int32 GetDOYfromMD(Int32 yr, Int32 month, Int32 day)
        {
            // could do this via datetime but what if there is an error in the month/day values that are untenable
            Int32[] nlyDays = new Int32[13] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31, 31 };
            Int32[] lyDays = new Int32[13] { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31, 31 };

            // first test the values and make sure they are good
            Int32[] days = nlyDays;
            if (EndingDOY(yr) == 366) days = lyDays;

            if (day < 1)
            {
                month--;
                if (month < 1)
                {
                    yr--;
                    month = 12;
                }
                day = days[month - 1] + day;
            }

            if (day > days[month - 1])
            {
                Int32 dysinpremon = days[month - 1];
                month++;
                if (month > 12)
                {
                    yr++;
                    month = 1;
                }
                day = day - dysinpremon;
            }

            // now convert to datetime and get the DOY
            DateTime date = new DateTime(yr, month, day);
            return date.DayOfYear;
        }

        public static String ConvertDatetoUTCDate(DateTime date)
        {
            // have to load everything and create the string from scratch since MS doesn't support the kinds of string I need to get out
            return (date.Year.ToString("D4") + "-" + date.DayOfYear.ToString("D3") +
                      "T" + date.Hour.ToString("D2") + ":" + date.Minute.ToString("D2") + ":" + date.Second.ToString("D2") + "." + date.Millisecond.ToString("D3"));
        }

        public static void ConvertParameterToProperty(string parameter, PropertyInfo prop, DataRecord obj)
        {
            TypeCode tc = Type.GetTypeCode(prop.PropertyType);//Convert.GetTypeCode(prop.PropertyType);

            switch (tc)
            {
                case TypeCode.DateTime:
                    prop.SetValue(obj, ConvertUTCtoDate(parameter), null);
                    return;
                case TypeCode.String:
                    prop.SetValue(obj, parameter, null);
                    return;
                case TypeCode.Int64:
                    prop.SetValue(obj, Convert.ToInt64(parameter), null);
                    return;
                case TypeCode.Int32:
                    prop.SetValue(obj, Convert.ToInt32(parameter), null);
                    return;
                case TypeCode.Int16:
                    prop.SetValue(obj, Convert.ToInt16(parameter), null);
                    return;
                case TypeCode.Byte:
                    prop.SetValue(obj, (byte)(Convert.ToInt16(parameter)), null);
                    return;
                case TypeCode.Boolean:
                    prop.SetValue(obj, Convert.ToBoolean(parameter), null);
                    return;
            }
        }
    }
}