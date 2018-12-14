using System;
using System.Collections.Generic;
using Nostreets_Services.Domain.Shopify;
using Nostreets_Services.Domain.Web;
using NostreetsExtensions.Extend.Web;
using NostreetsExtensions.Interfaces;
using RestSharp;

namespace Nostreets_Services.Services.Shopify
{
    public class ShopifyService
    {

        public ShopifyService(string domain, string apiKey)
        {
            Domain = domain;
            Headers.Add("authentication", apiKey);
            Headers.Add("contentType", "JSON");
        }

        public ShopifyService(string domain, string apiKey, IDBService<RequestError> errorSrv)
        {
            _errorSrv = errorSrv;
            Domain = domain;
            Headers.Add("authentication", apiKey);
            Headers.Add("contentType", "JSON");
        }

        private string Domain { get; set; }

        private Dictionary<string, string> Headers { get; set; }

        private IDBService<RequestError> _errorSrv = null;

        private IRestResponse<T> Endpoint<T>(string endpoint, string method = null, object data = null) where T : new()
        {
            string url = Domain + endpoint;
            return url.RestSharpEndpoint<T>(method, data, "application/json", Headers);
        }

        public List<Customer> GetCustomers()
        {
            try
            {
                IRestResponse<List<Customer>> response = Endpoint<List<Customer>>("/admin/customers.json", "GET");
                if (response.Data != null)
                    return response.Data;
                else
                    throw new Exception(response.ErrorMessage);
            }
            catch (Exception ex)
            {
                if (_errorSrv != null)
                    _errorSrv.Insert(new RequestError() { });

                throw ex;
            }
        }
    }
}
