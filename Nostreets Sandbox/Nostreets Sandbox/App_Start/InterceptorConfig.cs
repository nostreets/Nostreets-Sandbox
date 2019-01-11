using Nostreets_Sandbox.Classes;
using Nostreets_Services.Interfaces.Services;
using NostreetsExtensions.Extend.IOC;
using NostreetsExtensions.Extend.Web;
using NostreetsExtensions.Utilities;
using NostreetsInterceptor;
using NostreetsRouter.Models.Responses;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Web;

namespace Nostreets_Sandbox.App_Start
{
    public class InterceptorConfig
    {
        public InterceptorConfig()
        {
            _userSrv = _userSrv.WindsorResolve(WindsorConfig.GetContainer());
        }

        private IUserService _userSrv = null;

        #region Validators

        [Validator("LoggedIn")]
        private void LoggedIn(HttpApplication app)
        {
            if (_userSrv.SessionUser == null)
                Not_LoggedIn(app);
        }

        [Validator("ValidReferer")]
        private void ValidAPIReferer(HttpApplication app)
        {
            if (app.Context.Request.UrlReferrer.ToString().Contains(ConfigKeys.ServerDomain))
                Not_ValidAPIReferer(app);

        }

        #endregion

        #region Error Responses

        private void Not_LoggedIn(HttpApplication app)
        {
            app.Response.SetCookie(new HttpCookie("loggedIn", "false"));
            if (CacheManager.Contains("user"))
                CacheManager.Remove("user");

            app.CreateResponse(HttpStatusCode.Unauthorized, new ErrorResponse("User is not logged in..."));
        }

        private void Not_ValidAPIReferer(HttpApplication app)
        {
            app.CreateResponse(HttpStatusCode.Unauthorized, new ErrorResponse("API is private..."));
        }

        #endregion

    }
}
