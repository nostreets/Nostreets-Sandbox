using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using Nostreets_Services.Domain.Base;
using Nostreets_Services.Interfaces.Services;
using NostreetsExtensions;

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

        [Route]
        public ActionResult Index()
        {
            return View();
        }

        [Route("~/emailComfirmation")]
        public ActionResult EmailComfirmation(string token)
        {
            if (token != null)
            {
                Token userToken = JsonConvert.DeserializeObject<Token>(token);
                _userService.ValidateEmail(userToken);
            }
            return Redirect("/home");
        }
    }
}