using Nostreets_Sandbox.Classes;
using Nostreets_Services.Interfaces.Services;
using Nostreets.Extensions.Extend.IOC;
using Nostreets.Extensions.Extend.Web;
using Nostreets.Extensions.Utilities.Managers;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Web;
using Nostreets.Web.Interceptor;
using Nostreets.Web.Router.Models.Responses;

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

        [Validator("IsLoggedIn")]
        private void IsLoggedIn(HttpApplication app)
        {
            if (_userSrv.SessionUser == null)
                Not_LoggedIn(app);
        }

        [Validator("ValidReferer")]
        private void ValidUrlReferer(HttpApplication app)
        {
            if (app.Context.Request.UrlReferrer.ToString().Contains(ConfigKeys.ServerDomain))
                Not_ValidAPIReferer(app);

        }

        [Validator("IsBoardMember")]
        private void IsBoardMember(HttpApplication app)
        {
            if (!_userSrv.SessionUser.Roles.Contains("BoardMember"))
                Not_BoardMember(app);
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

        private void Not_BoardMember(HttpApplication app)
        {
            app.CreateResponse(HttpStatusCode.Unauthorized, new ErrorResponse("User is not eligible to hit OBL endpoints..."));
        }

        #endregion

    }
}
