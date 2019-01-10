using System;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using NostreetsExtensions.DataControl.Classes;

namespace Nostreets_Services.Domain.Web
{
    public class RequestError : Error
    {
        public RequestError() { }

        public RequestError(HttpRequestMessage request) : base()
        {
            Route = request.RequestUri.AbsoluteUri;
            Params = JsonConvert.SerializeObject(HttpUtility.ParseQueryString(request.RequestUri.Query));
        }

        public RequestError(HttpRequestMessage request, Exception ex) : base(ex)
        {
            Route = request.RequestUri.AbsoluteUri;
            Params = JsonConvert.SerializeObject(HttpUtility.ParseQueryString(request.RequestUri.Query));
        }

        public RequestError(HttpRequestMessage request, Exception ex, string data) : base(ex, data)
        {
            Route = request.RequestUri.AbsoluteUri;
            Params = JsonConvert.SerializeObject(HttpUtility.ParseQueryString(request.RequestUri.Query));
        }

        public RequestError(HttpRequestBase request) : base()
        {
            Route = request.Url.AbsoluteUri;
            Params = JsonConvert.SerializeObject(HttpUtility.ParseQueryString(request.Url.Query));
        }

        public RequestError(HttpRequestBase request, Exception ex) : base(ex)
        {
            Route = request.Url.AbsoluteUri;
            Params = JsonConvert.SerializeObject(HttpUtility.ParseQueryString(request.Url.Query));
        }

        public RequestError(HttpRequestBase request, Exception ex, string data) : base(ex, data)
        {
            Route = request.Url.AbsoluteUri;
            Params = JsonConvert.SerializeObject(HttpUtility.ParseQueryString(request.Url.Query));
        }

        public RequestError(string @params, string route, Exception ex) : base(ex)
        {
            Route = route;
            Params = @params;
        }

        public RequestError(string @params, string route, Exception ex, string data) : base(ex, data)
        {
            Route = route;
            Params = @params;
        }

        public string Params { get; set; }
        public string Route { get; set; }
    }
}
