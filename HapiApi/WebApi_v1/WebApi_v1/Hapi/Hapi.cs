using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using WebApi_v1.HAPI.Configuration;
using WebApi_v1.HAPI.Response;
using WebApi_v1.HAPI.Catalog;
using WebApi_v1.HAPI.Properties;
using WebApi_v1.HAPI.DataProducts;
using System.Net;
using System.Diagnostics;

namespace WebApi_v1.HAPI
{
    public class Hapi
    {
        public HapiConfiguration Configuration { get; set; }
        public HapiCatalog Catalog { get; set; }
        public HttpResponseMessage Response { get; set; }
        public HapiProperties Properties { get; set; }
        public HapiDataProduct DataProduct { get; set; }

        public Hapi()
        {
            Configuration = null;
            Catalog = null;
            Response = null;
            Properties = null;
            DataProduct = null;
        }

        public Hapi(HttpRequestMessage request)
        {
            Configure(request);
        }
        
        public void Configure(HttpRequestMessage request)
        {
            Configuration = new HapiConfiguration();
            Configuration.ParseRequest(request);

            Catalog = new HapiCatalog();
            Catalog.Create();

            Properties = new HapiProperties();
            Properties.Assign(request, Configuration, Catalog);
        }

        public HttpResponseMessage GetResponse()
        {
            if (Properties == null)
                throw new InvalidOperationException("Hapi.Properties not set.");

            Content content = null;
            switch (Properties.RequestType.ToLower())
            {
                case ("data"):
                    if (Properties.ErrorCodes.Count() == 0 && GetDataProduct())
                        content = Content.Create(this, "data");
                    break;

                case ("catalog"):
                    ;
                    content = Content.Create(this, "catalog");
                    break;

                case ("info"):
                    content = Content.Create(this, "info");
                    break;

                case ("capabilities"):
                    content = Content.Create(this, "capabilities");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(Properties.RequestType, "Not a valid request type.");
            };

            if (Properties.ErrorCodes.Count() == 0 && content != null)
            {
                Response = Properties.Request.CreateResponse(HttpStatusCode.OK);
                content.SetStatusCode(Status.HapiStatusCode.OK);
                Response.Content = new StringContent(content.GetResponse());
            }
            else
            {
                content = Content.Create(this, "error");
                Response = Properties.Request.CreateResponse(HttpStatusCode.BadRequest);
                content.SetStatusCode(Properties.ErrorCodes.First());
                Response.Content = new StringContent(content.GetResponse());
            }

            return Response;
        }

        private bool GetDataProduct()
        {
            try
            {
                DataProduct = HapiDataProduct.Create(Properties.SC, this);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Properties.ErrorCodes.Add(Status.HapiStatusCode.InternalServerError);
            }

            if (DataProduct == null)
            {

                return false;
            }

            if (!DataProduct.VerifyTimeRange()) // Outside of SC data timerange
            {
                Properties.ErrorCodes.Add(Status.HapiStatusCode.TimeOutsideValidRange);
                return false;
            }
            else if (!DataProduct.GetProduct()) // Data doesn't exist
            {
                Properties.ErrorCodes.Add(Status.HapiStatusCode.OKNoDataForTimeRange);
                return false;
            }

            return true;
        }
    }
}