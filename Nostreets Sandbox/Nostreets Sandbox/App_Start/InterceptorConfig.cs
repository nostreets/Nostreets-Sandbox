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
using System.Web.SessionState;

namespace Nostreets_Sandbox.App_Start
{
    public class InterceptorConfig
    {
        public InterceptorConfig()
        {
            _userSrv = _userSrv.WindsorResolve(WindsorConfig.GetContainer());
        }

        IUserService _userSrv = null;

        [Validator("Authorization")]
        void NeedsToBeAuthorized(HttpApplication app)
        {
            if (_userSrv.SessionUser == null)
                NotAuthorized(app); 

            //HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)
            if(app.Context.Request.UrlReferrer.ToString().Contains(ConfigKeys.ServerDomain))
                NotAuthorized(app);

        }


        void NotAuthorized(HttpApplication app)
        {
            app.Response.SetCookie(new HttpCookie("loggedIn", "false"));
            if (CacheManager.Contains("user"))
            {
                CacheManager.Remove("user");
            }

            app.CreateResponse(HttpStatusCode.Unauthorized, new ErrorResponse("User is not logged in..."));
        }


    }
}
