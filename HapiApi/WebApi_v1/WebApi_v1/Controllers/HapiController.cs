using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi_v1.Models;
using WebApi_v1.DataProducts;
using Newtonsoft.Json;
using System.Diagnostics;
using WebApi_v1.DataProducts.Utilities;
using System.ComponentModel;
using WebApi_v1.DataProducts.RBSpiceA;
using System.IO;

namespace WebApi_v1.Controllers
{
    public class HapiController : ApiController
    {

        [Route("api/Hapi/Info")]
        [HttpGet]
        public IProduct GetInfo()
        {
            Hapi.Configure(Request);
            Debug.WriteLine(Hapi.ToString);

            IProduct prod = new RBSpiceAProduct();
            return prod;
        }


        [Route("api/Hapi/Data")]
        [HttpGet]
        public HttpResponseMessage GetData()//, [FromUri] string id=null,[FromUri] DateTime? starttime=null, [FromUri] DateTime? endtime=null, [FromUri] string subseconds=null)
        {
            if (Hapi.Configure(Request))
                Debug.WriteLine(Hapi.ToString);

            if (Hapi.CreateResponse())
                return Hapi.Response;
            else
            {
                return new HttpResponseMessage((HttpStatusCode)500);
            }
        }

        [Route("api/Hapi/Catalog")]
        [HttpGet]
        public IProduct GetCatalog()//, [FromUri] string id=null,[FromUri] DateTime? starttime=null, [FromUri] DateTime? endtime=null, [FromUri] string subseconds=null)
        {

            Hapi.Configure(Request);
            Debug.WriteLine(Hapi.ToString);


            IProduct prod = new RBSpiceAProduct();
            return prod;
        }


        [Route("api/Hapi/Capabilities")]
        [HttpGet]
        public IProduct GetCapabilities()//, [FromUri] string id=null,[FromUri] DateTime? starttime=null, [FromUri] DateTime? endtime=null, [FromUri] string subseconds=null)
        {
            if (Hapi.Configure(Request))
                Debug.WriteLine(Hapi.ToString);
            else
                Debug.WriteLine(Hapi.Properties.Error.Message);


            IProduct prod = new RBSpiceAProduct();
            return prod;
        }
    }
}
