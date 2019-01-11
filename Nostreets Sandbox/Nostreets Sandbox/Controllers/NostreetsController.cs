using System.Web.Mvc;
using Nostreets_Services.Interfaces.Services;
using NostreetsInterceptor;
using NostreetsExtensions.DataControl.Classes;
using NostreetsExtensions.Extend.IOC;
using NostreetsExtensions.DataControl.Enums;
using Nostreets_Services.Classes.Domain.Users;

namespace Nostreets_Sandbox.Controllers
{
    public class NostreetsController : Controller
    {
        public NostreetsController()
        {
            _userService = _userService.WindsorResolve(App_Start.WindsorConfig.GetContainer());
        }

        public IUserService _userService = null;


        public ActionResult Index(string token = null, string user = null)
        {
            User sessionUser = null;
            Token userToken = null;
            string outcome = null;
            bool hasVisited = false;
            State state = State.Error;

            if (token != null && user != null)
                userToken = _userService.ValidateToken(new TokenRequest { TokenId = token, UserId = user }, out state, out outcome);

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
                hasVisited,
                state = state.ToString()
            };


            return View();
        }

        public ActionResult PrivatePolicy()
        {
            return View();
        }

    }
}