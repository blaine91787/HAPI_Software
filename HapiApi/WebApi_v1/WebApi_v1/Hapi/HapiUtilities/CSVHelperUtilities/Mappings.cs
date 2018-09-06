using CsvHelper.Configuration;
using WebApi_v1.HAPI.DataProducts.SpaceCraft.RBSPA.RBSpice.Auxiliary;

namespace WebApi_v1.HAPI.Utilities.CSVHelperUtilities
{
    public static class Mappings
    {
        /// <summary>
        /// A mapping used by CSVHelper library to convert the CSV version of time to DateTime
        /// so it can be compared when searching for time.min and time.max parameters.
        /// </summary>
        public sealed class Aux_Map : ClassMap<Auxiliary>
        {
            public Aux_Map()
            {
                AutoMap();
                Map(m => m.UTC).Name("UTC").TypeConverter<TypeConverters.ConvertUTCtoDateTime>();
            }
        }
    }
}