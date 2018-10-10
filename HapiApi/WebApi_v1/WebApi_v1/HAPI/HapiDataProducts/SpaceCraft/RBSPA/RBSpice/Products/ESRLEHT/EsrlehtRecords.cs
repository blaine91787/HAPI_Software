using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_v1.HAPI.HapiDataProducts.SpaceCraft.RBSPA.RBSpice.Products.ESRLEHT
{
    public class EsrlehtRecords : ProductRecords
    {
        public EsrlehtRecords(Hapi hapi)
        {
            if (hapi != null)
                Hapi = hapi;
        }
    }
}