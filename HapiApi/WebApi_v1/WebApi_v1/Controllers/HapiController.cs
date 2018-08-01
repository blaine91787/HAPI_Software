using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi_v1.DataProducts;

namespace WebApi_v1.Controllers
{
    public class HapiController : ApiController
    {
        [Route("api/Hapi/Info")]
        [HttpGet]
        public HttpResponseMessage GetInfo()
        {
            HapiConfiguration hapi = new HapiConfiguration();
            hapi.Configure(Request);
            hapi.CreateResponse();

            return hapi.Response;
        }

        [Route("api/Hapi/Data")]
        [HttpGet]
        public HttpResponseMessage GetData()
        {
            HapiConfiguration hapi = new HapiConfiguration();
            if (!hapi.Configure(Request))
            {
                foreach (Exception e in hapi.Errors)
                    Debug.WriteLine(e.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            if (!hapi.CreateResponse())
            {
                foreach (Exception e in hapi.Errors)
                    Debug.WriteLine(e.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            return hapi.Response;
        }

        [Route("api/Hapi/Catalog")]
        [HttpGet]
        public HttpResponseMessage GetCatalog()
        {
            HapiConfiguration hapi = new HapiConfiguration();
            hapi.Configure(Request);

            hapi.CreateResponse();

            return hapi.Response;
        }

        [Route("api/Hapi/Capabilities")]
        [HttpGet]
        public HttpResponseMessage GetCapabilities()
        {
            HapiConfiguration hapi = new HapiConfiguration();
            hapi.Configure(Request);
            hapi.CreateResponse();
            return hapi.Response;
        }
    }
}