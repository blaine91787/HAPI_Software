using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;

namespace WebApi_v1.DataProducts.Utilities.CSVHelperUtilities
{
    public static class TypeConverters
    {
        public class ConvertUTCtoDateTime : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                return Converters.ConvertUTCtoDate(text);
            }

            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                return Converters.ConvertDatetoUTCDate((DateTime)value);
            }
        }
    }
}