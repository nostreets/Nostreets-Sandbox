using Nostreets_Services.Domain;
using Nostreets_Services.Interfaces.Services;
using Nostreets_Services.Utilities;
using NostreetsExtensions;
using NostreetsInterceptor;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Web;

namespace Nostreets_Services.Services.Database
{
    public class InterceptorConfig
    {
        public InterceptorConfig(IUserService userInject)
        {
            _userSrv = userInject;
        }

        IUserService _userSrv = null;

        [Validator("UserLogIn")]
        void GetCurrentUser(HttpApplication app)
        {
            if (app.Request.GetCookie("loggedIn") == null || app.Request.GetCookie("loggedIn") == "false") { NotLoggedIn(app); }
            else
            {
                string uid = CacheManager.GetItem<string>("uid");
                if (uid == null) { NotLoggedIn(app); }

                User user = _userSrv.Get(uid);
                if (user == null) { NotLoggedIn(app); }
            }
        }

        void NotLoggedIn(HttpApplication app)
        {
            app.Response.SetCookie(new HttpCookie("loggedIn", "false"));
            if (CacheManager.Contains("uid"))
            {
                CacheManager.DeleteItem("uid");
            }

            app.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            app.Response.End();
        }
        
    }
}
