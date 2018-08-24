using System.Web.Mvc;
using Nostreets_Services.Domain;
using Nostreets_Services.Interfaces.Services;
using NostreetsInterceptor;
using NostreetsExtensions.DataControl.Classes;
using NostreetsExtensions.Extend.IOC;

namespace Nostreets_Sandbox.Controllers
{
    [RoutePrefix("home")]
    public class HomeController : Controller
    {
        public HomeController()
        {
            _userService = _userService.WindsorResolve(App_Start.WindsorConfig.GetContainer());
        }

        public IUserService _userService = null;

        [Route("~/"), Intercept("LoggedIn")]
        public ActionResult Index(string token = null, string user = null)
        {
            User sessionUser = null;
            Token userToken = null;
            string outcome = null;
            bool hasVisited = false;

            if (token != null && user != null)
                userToken = _userService.ValidateToken(new TokenRequest { TokenId = token, UserId = user }, out outcome);

            if (_userService.SessionUser != null)
            {
                sessionUser = _userService.SessionUser;
                hasVisited = true;
            }

            //else
            //    hasVisited  = (_userService.FirstOrDefault(a => a.Settings.IPAddresses != null && a.Settings.IPAddresses.Any(b => b == _userService.RequestIp)) != null) 
            //        ? true : false;





            ViewBag.ServerModel = new
            {
                user = sessionUser,
                token = userToken,
                tokenOutcome = outcome,
                hasVisited
            };

            return View();
        }

        public ActionResult PrivatePolicy()
        {
            return View();
        }

    }
}