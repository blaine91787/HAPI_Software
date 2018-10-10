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
using WebApi_v1.HAPI.Registry;
using System.Text;
using System.Runtime.InteropServices;

namespace WebApi_v1.HAPI
{
    /// <summary>
    /// The main Hapi class.
    /// Everything begins from here. 
    /// </summary>
    /// <remarks>
    /// Order: Configure, set Hapi.Properties from request query, and create response.
    /// </remarks>
    public class Hapi
    {
        public static HapiRegistry Registry = new HapiRegistry();
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
        /// <summary>
        /// Configuration is populated with Hapi spec info and relevant paths.
        /// Catalog is populated with available spacecraft/instruments/products.
        /// Properties are assigned based on the user query. Anything pulled from query is saved in Hapi.Properties.
        /// </summary>
        /// <param name="request">
        /// A System.Net.Http Request created by the API
        /// </param>
        public void Configure(HttpRequestMessage request)
        {
            Configuration = new HapiConfiguration();
            Configuration.Initialize();

            Catalog = new HapiCatalog();
            Catalog.Create(Configuration.Paths);

            Properties = new HapiProperties();
            Properties.Assign(request, Configuration, Catalog);
        }
        /// <summary>
        /// Based on Properties.RequestType.
        /// A Response will be created using the Content abstract class factory, then added to the Hapi.Response property, and returned to the user
        /// </summary>
        /// <returns>
        /// The response message created by Hapi
        /// </returns>
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
                if (Properties.Format == "json")
                    Response.Content = new StringContent(content.GetResponse(), Encoding.UTF8, "application/json");
                else if (Properties.Format == "csv")
                    Response.Content = new StringContent(content.GetResponse(), Encoding.UTF8, "text/csv");
            }
            else
            {
                content = Content.Create(this, "error");
                Response = Properties.Request.CreateResponse(HttpStatusCode.BadRequest);
                content.SetStatusCode(Properties.ErrorCodes.First());
                Response.Content = new StringContent(content.GetResponse(), Encoding.UTF8, "application/json");
            }
            return Response;
        }
        /// <summary>
        /// Uses the HapiDataProduct abstract class factory to create dataproduct based on SC name.
        /// If there are errors with the time range requested vs what's available error responses are recorded.
        /// If the TimeRange is valid but there's no data for the requested time range, error codes recorded.
        /// </summary>
        /// <returns>
        /// True if there are no errors in creating the dataproduct, false otherwise.
        /// </returns>
        private bool GetDataProduct()
        {
            try
            {
                DataProduct = HapiDataProduct.Create(Properties.SC, this);
            }
            catch (Exception e) // A few things can cause errors here, check HapiDataProduct.Create() for exceptions
            {
                Debug.WriteLine(e.Message);
                Properties.ErrorCodes.Add(Status.HapiStatusCode.InternalServerError);
            }

            //if (DataProduct.Records == null)
            //    throw new InvalidOperationException("DataProduct.Records should not come back null, something happened.");

            if (!DataProduct.VerifyTimeRange()) // Outside of SC data timerange
            {
                DateTime min = Properties.TimeRange.UserMin;
                DateTime max = Properties.TimeRange.UserMax;
                DateTime availMin = Properties.TimeRange.Min;
                DateTime availMax = Properties.TimeRange.Max;

                // Requested start time >= Requested end time
                if (min >= max)
                    Properties.ErrorCodes.Add(Status.HapiStatusCode.StartTimeEqualToOrAfterStopTime);

                // Requested start time <= available min OR requested end time >= available max
                if (min <= availMin || max >= availMax)
                    Properties.ErrorCodes.Add(Status.HapiStatusCode.TimeOutsideValidRange);
                return false;
            }
            else // Data doesn't exist
            {
                try
                {
                    if(!DataProduct.GetProduct())
                    {
                        Properties.ErrorCodes.Add(Status.HapiStatusCode.OKNoDataForTimeRange);
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Properties.ErrorCodes.Add(Status.HapiStatusCode.InternalServerError);
                    Debug.WriteLine(e.Message);
                    return false;
                }
            }

            return true;
        }
    }
}