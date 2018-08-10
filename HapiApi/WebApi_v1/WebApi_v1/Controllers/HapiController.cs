using System.Net.Http;
using System.Web.Http;
using WebApi_v1.Hapi;

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
            return hapi.GetResponse();
        }

        [Route("api/Hapi/Data")]
        [HttpGet]
        public HttpResponseMessage GetData()
        {
            HapiConfiguration hapi = new HapiConfiguration();
            hapi.Configure(Request);
            return hapi.GetResponse(); 
        }

        [Route("api/Hapi/Catalog")]
        [HttpGet]
        public HttpResponseMessage GetCatalog()
        {
            HapiConfiguration hapi = new HapiConfiguration();
            hapi.Configure(Request);
            return hapi.GetResponse();
        }

        [Route("api/Hapi/Capabilities")]
        [HttpGet]
        public HttpResponseMessage GetCapabilities()
        {
            HapiConfiguration hapi = new HapiConfiguration();
            hapi.Configure(Request);
            return hapi.GetResponse();
        }
    }
}