using System;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;

using Nostreets.Extensions.DataControl.Classes;


namespace Nostreets_Services.Classes.Domain.Web
{
    public class WebRequestError : Error
    {
        public WebRequestError() { }

        public WebRequestError(HttpRequestMessage request) : base()
        {
            Route = request.RequestUri.AbsoluteUri;
            Params = JsonConvert.SerializeObject(HttpUtility.ParseQueryString(request.RequestUri.Query));
        }

        public WebRequestError(HttpRequestMessage request, Exception ex) : base(ex)
        {
            Route = request.RequestUri.AbsoluteUri;
            Params = JsonConvert.SerializeObject(HttpUtility.ParseQueryString(request.RequestUri.Query));
        }

        public WebRequestError(HttpRequestMessage request, Exception ex, string data) : base(ex, data)
        {
            Route = request.RequestUri.AbsoluteUri;
            Params = JsonConvert.SerializeObject(HttpUtility.ParseQueryString(request.RequestUri.Query));
        }

        public WebRequestError(HttpRequestBase request) : base()
        {
            Route = request.Url.AbsoluteUri;
            Params = JsonConvert.SerializeObject(HttpUtility.ParseQueryString(request.Url.Query));
        }

        public WebRequestError(HttpRequestBase request, Exception ex) : base(ex)
        {
            Route = request.Url.AbsoluteUri;
            Params = JsonConvert.SerializeObject(HttpUtility.ParseQueryString(request.Url.Query));
        }

        public WebRequestError(HttpRequestBase request, Exception ex, string data) : base(ex, data)
        {
            Route = request.Url.AbsoluteUri;
            Params = JsonConvert.SerializeObject(HttpUtility.ParseQueryString(request.Url.Query));
        }

        public WebRequestError(string @params, string route, Exception ex) : base(ex)
        {
            Route = route;
            Params = @params;
        }

        public WebRequestError(string @params, string route, Exception ex, string data) : base(ex, data)
        {
            Route = route;
            Params = @params;
        }

        public string Params { get; set; }
        public string Route { get; set; }
    }
}
