using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_v1.DataProducts
{

    public abstract class DataRecord : IDataRecord
    {
        public DateTime UTC;
        public override string ToString()
        {
            string str = "";

            foreach (System.Reflection.PropertyInfo prop in this.GetType().GetProperties())
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                str += prop.GetValue(this, null).ToString() + ", ";
            }

            return str;
        }
    }
}