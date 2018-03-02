using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using Nostreets_Services.Domain.Base;
using Nostreets_Services.Interfaces.Services;

namespace Nostreets_Sandbox.Controllers
{
    [RoutePrefix("home")]
    public class HomeController : Controller
    {
        public HomeController(IUserService userSrv)
        {
            _userService = userSrv;
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