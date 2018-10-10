using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_v1.HAPI.HapiDataProducts.SpaceCraft.RBSPA.RBSpice.Products.TOFxEIon
{
    public class TofxeionRecords : ProductRecords
    {
        public TofxeionRecords(Hapi hapi)
        {
            if (hapi != null)
                Hapi = hapi;
        }
    }
}