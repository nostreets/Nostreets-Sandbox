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
        [Microsoft.Practices.Unity.Dependency]
        public IUserService UserService { get; set; }

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
                UserService.ValidateEmail(userToken);
            }
            return Redirect("/home");
        }
    }
}