using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi_v1.DataProducts;
using WebApi_v1.DataProducts.RBSpiceA;

namespace WebApi_v1.Controllers
{
    public class HapiController : ApiController
    {
        [Route("api/Hapi/Info")]
        [HttpGet]
        public HttpResponseMessage GetInfo()
        {
            Hapi.Configure(Request);
            Hapi.CreateResponse();

            return Hapi.Response;
        }

        [Route("api/Hapi/" +
            "Data")]
        [HttpGet]
        public HttpResponseMessage GetData()
        {
            // TODO: Check if this should be multithreaded
            if (!Hapi.Configure(Request))
            {
                foreach (Exception e in Hapi.Errors)
                    Debug.WriteLine(e.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            if (!Hapi.CreateResponse())
            {
                foreach (Exception e in Hapi.Errors)
                    Debug.WriteLine(e.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            return Hapi.Response;
        }

        [Route("api/Hapi/Catalog")]
        [HttpGet]
        public HttpResponseMessage GetCatalog()
        {
            Hapi.Configure(Request);

            Hapi.CreateResponse();

            return Hapi.Response;
        }

        [Route("api/Hapi/Capabilities")]
        [HttpGet]
        public HttpResponseMessage GetCapabilities()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Accepted);
            response.Content = new StringContent(new Hapi.CapabilitiesResponse().ToString());
            return response;
        }
    }
}