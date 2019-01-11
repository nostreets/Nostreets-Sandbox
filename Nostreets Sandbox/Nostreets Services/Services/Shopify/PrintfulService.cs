using Nostreets_Services.Classes.Domain.Web;
using Nostreets_Services.Interfaces.Services;
using NostreetsExtensions.Extend.Web;
using NostreetsExtensions.Interfaces;
using RestSharp;
using System;
using System.Collections.Generic;

namespace Nostreets_Services.Services.Shopify
{
    public class PrintfulService
    {
        public PrintfulService(string domain, string apiKey)
        {
            Domain = domain;
            ApiKey = apiKey;
        }

        public string Domain { get; set; }
        public string ApiKey { get; set; }
        private Dictionary<string, string> Headers { get; set; }

        private IDBService<WebRequestError> _errorLog = null;
        private IUserService _userSrv = null;
        private IRestResponse<T> Endpoint<T>(string endpoint, string method = null, object data = null) where T : new()
        {
            string url = Domain + endpoint;
            return url.RestSharpEndpoint<T>(method, data, "application/json", Headers);
        }

        public List<object> GetOrders(string status = null, string offset = null, string limit = null)
        {

            try
            {
                IRestResponse<List<object>> response = Endpoint<List<object>>("/orders", "GET");

                if (response.IsSuccessful)
                    return response.Data;
                else
                    throw new Exception(response.ErrorMessage);
            }
            catch (Exception ex)
            {
                if (_errorLog != null)
                    _errorLog.Insert(new WebRequestError(null, Domain + "/admin/customers.json", ex));

                throw ex;
            }
        }

    }
}
