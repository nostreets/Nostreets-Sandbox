using Newtonsoft.Json;
using NostreetsExtensions;
using NostreetsExtensions.Extend.Basic;
using NostreetsExtensions.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Nostreets_Sandbox.Classes
{
    public class AuthByKeyAttribute : AuthorizeAttribute
    {
        public AuthByKeyAttribute() { _connectionKey = "DefaultConnection"; }
        public AuthByKeyAttribute(string connectionKey) { _connectionKey = connectionKey; }

        private string _connectionKey;

        private bool Authorize(HttpActionContext actionContext)
        {
            string authKey = null;
            string ip = GetRequestIPAddress();
            Dictionary<string, string> whitelistedIps = JsonConvert.DeserializeObject<Dictionary<string, string>>(WebConfigurationManager.AppSettings["WhitelistedIps"]);

            if (actionContext.Request.RequestUri.Host == "localhost" || whitelistedIps.ContainsValue(ip))
            {
                authKey = WebConfigurationManager.AppSettings["Sandbox.Azure.ApiKey"];
            }
            if (actionContext.Request.Headers.Authorization == null && authKey == null) { return false; }
            if (authKey == null) { authKey = actionContext.Request.Headers.Authorization.Parameter; }

            bool isTrue = false;
            DataProvider.SqlInstance.ExecuteCmd(() => new SqlConnection(WebConfigurationManager.ConnectionStrings[_connectionKey].ConnectionString), "dbo.CheckApiKey",
                (a) =>
                {
                    a.Add(new SqlParameter("key", authKey));
                },
                (reader, set) => { isTrue = DataMapper.MapToObject<bool>(reader); });
            return isTrue;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (!Authorize(actionContext))
            {
                HandleUnauthorizedRequest(actionContext);
            }
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized Request");
            return;
        }

        private string GetRequestIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
    }

    public class AuthByPredicateAttribute : AuthorizeAttribute
    {
        public AuthByPredicateAttribute(MethodInfo predicate, Type delegateType)
        {
            if (!delegateType.IsSubclassOf(typeof(Delegate)) || delegateType != typeof(Func<bool>) || delegateType != typeof(Func<bool>))
            {
                throw new InvalidOperationException(delegateType.Name + " is not supported delegate type");
            }
            else if (delegateType == typeof(Func<HttpActionContext, bool>))
            {
                _predicateWithInput = (Func<HttpActionContext, bool>)predicate.ToDelegate();
            }
            else if (delegateType != typeof(Func<bool>))
            {
                _predicateWithoutInput = (Func<bool>)predicate.ToDelegate();
            }

        }


        private Func<HttpActionContext, bool> _predicateWithInput = null;
        private Func<bool> _predicateWithoutInput = null;

        private bool Authorize(HttpActionContext actionContext)
        {
            return (_predicateWithInput == null) ? _predicateWithInput(actionContext) : _predicateWithoutInput();
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (!Authorize(actionContext))
            {
                HandleUnauthorizedRequest(actionContext);
            }
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized Request");
            return;
        }

    }
}