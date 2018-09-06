using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;

namespace WebApi_v1.HAPI.Utilities.CSVHelperUtilities
{
    public class TypeConverters
    {
        public class ConvertUTCtoDateTime : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                Converters cons = new Converters();
                return cons.ConvertUTCtoDate(text);
            }

            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                Converters cons = new Converters();
                return cons.ConvertDatetoUTCDate((DateTime)value);
            }
        }
    }
}