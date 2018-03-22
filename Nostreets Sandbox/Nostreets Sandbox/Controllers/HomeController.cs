using System;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Nostreets_Services.Domain;
using Nostreets_Services.Domain.Base;
using Nostreets_Services.Interfaces.Services;
using NostreetsExtensions;
using NostreetsExtensions.Utilities;

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

        [Route("~/")]
        public ActionResult Index(string token = null, string userId = null)
        {
            User user = null;
            Token userToken = null;
            string outcome = "";

            if (_userService.SessionUser != null)
                user = _userService.SessionUser;

            else
            {
                string userIp = HttpContext.Request.UserHostAddress;
                user = _userService.Where(a => a.Settings.IPAddresses != null && a.Settings.IPAddresses.Any(b => b == userIp)).FirstOrDefault();

            }


            if (token != null && userId != null)
                outcome = _userService.ValidateToken(token, userId);


            ViewBag.TokenState = new
            {
                type = userToken?.Type.ToString(),
                outcome = outcome
            };

            return View(user);
        }

    }
}