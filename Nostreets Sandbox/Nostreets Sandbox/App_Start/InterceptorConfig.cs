﻿using Nostreets_Services.Domain;
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

        [Validator("Session")]
        void AddSessionCookie(HttpApplication app)
        {
            //app.Context.SetSessionStateBehavior(SessionStateBehavior.Required);
        }

        [Validator("UserLogIn")]
        void GetCurrentUser(HttpApplication app)
        {
            if (!SessionManager.HasAnySessions() || !SessionManager.Get<bool>(SessionState.IsLoggedOn)) { NotLoggedIn(app); }
            else
            {
                string userId = CacheManager.GetItem<string>(HttpContext.Current.GetIPAddress());
                if (userId == null) { NotLoggedIn(app); }
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
