using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_v1.HAPI.HapiDataProducts.SpaceCraft.RBSPA.RBSpice.Products.TOFxPHHLEHT
{
    public class TofxphhlehtRecords : ProductRecords
    {
        public TofxphhlehtRecords(Hapi hapi)
        {
            if (hapi != null)
                Hapi = hapi;
        }
    }
}