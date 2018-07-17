using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApi_v1.DataProducts.Utilities;

namespace WebApi_v1.DataProducts.CSVHelperUtilities
{
    public class TypeConverters
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