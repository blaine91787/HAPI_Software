using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_v1.HAPI.HapiDataProducts.SpaceCraft.RBSPA.RBSpice.Products.TOFxEHe
{
    public class TofxeheRecords : ProductRecords
    {
        public TofxeheRecords(Hapi hapi)
        {
            if (hapi != null)
                Hapi = hapi;
        }
    }
}