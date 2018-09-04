using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using WebApi_v1.HAPI;

namespace WebApi_v1.Controllers
{
    public class HapiController : ApiController
    {
        [Route("api/Hapi/Info")]
        [HttpGet]
        public HttpResponseMessage GetInfo()
        {
            Hapi hapi = new Hapi(Request);
            return hapi.GetResponse();
        }

        [Route("api/Hapi/Data")]
        [HttpGet]
        public HttpResponseMessage GetData()
        {
            Hapi hapi = new Hapi(Request);
            return hapi.GetResponse(); 
        }

        [Route("api/Hapi/Catalog")]
        [HttpGet]
        public HttpResponseMessage GetCatalog()
        {
            Hapi hapi = new Hapi(Request);
            return hapi.GetResponse();
        }

        [Route("api/Hapi/Capabilities")]
        [HttpGet]
        public HttpResponseMessage GetCapabilities()
        {
            Hapi hapi = new Hapi(Request);
            return hapi.GetResponse();
        }
    }
}