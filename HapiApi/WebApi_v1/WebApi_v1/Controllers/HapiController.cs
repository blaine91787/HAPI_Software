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
        public IProduct GetInfo()
        {
            Hapi.Configure(Request);
            Debug.WriteLine(Hapi.ToString);

            IProduct prod = new RBSpiceAProduct(Hapi.Properties);
            return prod;
        }

        [Route("api/Hapi/Data")]
        [HttpGet]
        public HttpResponseMessage GetData()
        {
            // TODO: Check if this should be multithreaded
            if (!Hapi.Configure(Request))
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            if (!Hapi.CreateResponse())
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            return Hapi.Response;
        }

        [Route("api/Hapi/Catalog")]
        [HttpGet]
        public IProduct GetCatalog()
        {
            Hapi.Configure(Request);
            Debug.WriteLine(Hapi.ToString);

            IProduct prod = new RBSpiceAProduct(Hapi.Properties);
            return prod;
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