using Nostreets_Services.Interfaces.Services;
using NostreetsExtensions;
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

        IUserService _userSrv = null;

        [Validator("UserLogIn")]
        void GetCurrentUser(HttpApplication app)
        {
            if (SessionManager.Get<bool>(SessionState.IsLoggedOn)/*app.Request.GetCookie("loggedIn") == null || app.Request.GetCookie("loggedIn") == "false"*/) { NotLoggedIn(app); }
            else
            {
                //User user = CacheManager.GetItem<User>("user");
                bool isLoggedOn = SessionManager.Get<bool>(SessionState.IsLoggedOn);
                if (!isLoggedOn) { NotLoggedIn(app); }
            }
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
