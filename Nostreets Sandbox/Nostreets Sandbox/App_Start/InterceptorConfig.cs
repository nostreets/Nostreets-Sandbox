using Nostreets_Services.Domain;
using Nostreets_Services.Interfaces.Services;
using NostreetsExtensions;
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

        [Validator("LoggedIn")]
        void NeedsToBeLoggedIn(HttpApplication app)
        {
            if (_userSrv.SessionUser == null)
                NotLoggedIn(app); 
        }

        void NotLoggedIn(HttpApplication app)
        {
            app.Response.SetCookie(new HttpCookie("loggedIn", "false"));
            if (CacheManager.Contains("user"))
            {
                CacheManager.DeleteItem("user");
            }

            app.CreateResponse(HttpStatusCode.Unauthorized, new ErrorResponse("User is not logged in..."));
        }


    }
}
