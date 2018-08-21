using System.Net.Http;
using System.Web.Http;

namespace WebApi_v1.Controllers
{
    public class HapiController : ApiController
    {
        [Route("api/Hapi/Info")]
        [HttpGet]
        public HttpResponseMessage GetInfo()
        {
            Hapi.Configuration hapi = new Hapi.Configuration();
            hapi.Configure(Request);
            return hapi.GetResponse();
        }

        [Route("api/Hapi/Data")]
        [HttpGet]
        public HttpResponseMessage GetData()
        {
            Hapi.Configuration hapi = new Hapi.Configuration();
            hapi.Configure(Request);
            return hapi.GetResponse(); 
        }

        [Route("api/Hapi/Catalog")]
        [HttpGet]
        public HttpResponseMessage GetCatalog()
        {
            Hapi.Configuration hapi = new Hapi.Configuration();
            hapi.Configure(Request);
            return hapi.GetResponse();
        }

        [Route("api/Hapi/Capabilities")]
        [HttpGet]
        public HttpResponseMessage GetCapabilities()
        {
            Hapi.Configuration hapi = new Hapi.Configuration();
            hapi.Configure(Request);
            return hapi.GetResponse();
        }
    }
}