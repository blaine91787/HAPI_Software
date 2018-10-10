using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_v1.HAPI.HapiDataProducts.SpaceCraft.RBSPA.RBSpice.Products.TOFxEO
{
    public class TofxeoRecords : ProductRecords
    {
        public TofxeoRecords(Hapi hapi)
        {
            if (hapi != null)
                Hapi = hapi;
        }
    }
}