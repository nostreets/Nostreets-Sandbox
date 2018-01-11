using NostreetsRouter.Models.Responses;
using Nostreets_Services.Domain;
using Nostreets_Services.Domain.Bills;
using Nostreets_Services.Domain.Cards;
using Nostreets_Services.Domain.Charts;
using Nostreets_Services.Enums;
using Nostreets_Services.Interfaces.Services;
using Nostreets_Services.Models.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using NostreetsInterceptor;
using NostreetsExtensions;
using NostreetsExtensions.Utilities;
using NostreetsExtensions.Interfaces;
using Microsoft.Practices.Unity;

namespace Nostreets_Sandbox.Controllers.Api
{
    [RoutePrefix("api")]
    public class SandoxApiController : ApiController
    {
        #region Global Objects
        IChartsExtended _chartsSrv = null;
        ISendGridService _sendGridSrv = null;
        IDBService<StyledCard> _cardSrv = null;
        IUserService _userSrv = null;
        IBillService _billSrv = null;
        #endregion

        public SandoxApiController(IChartsExtended chartsInject, ISendGridService sendGridInject, IDBService<StyledCard> cardInject, IUserService userInject, IBillService billsInject)
        {
            _chartsSrv = /*App_Start.UnityConfig.GetContainer().Resolve<Nostreets_Services.Services.Database.ChartsService>(); //*/chartsInject;
            _sendGridSrv = /*App_Start.UnityConfig.GetContainer().Resolve<Nostreets_Services.Services.Web.SendGridService>(); //*/sendGridInject;
            _cardSrv = /*App_Start.UnityConfig.GetContainer().Resolve<NostreetsORM.DBService<StyledCard>>(); //*/cardInject;
            _userSrv = /*App_Start.UnityConfig.GetContainer().Resolve<Nostreets_Services.Services.Database.UserService>(); //*/userInject;
            _billSrv = /*App_Start.UnityConfig.GetContainer().Resolve<Nostreets_Services.Services.Database.BillService>(); //*/billsInject;

        }

        #region Private Members

        private string GetStringWithinLines(int begining, int ending, string[] file)
        {

            string result = null;
            for (int i = begining - 1; i <= ending; i++)
            {
                result += "\r\n" + file[i];
            }
            return result;
        }

        private User CurrentUser { get { return CacheManager.GetItem<User>("user"); } }


        #endregion

        #region User Service Endpoints
        [Route("user")]
        [HttpGet]
        public HttpResponseMessage LogInUser(string username)
        {
            try
            {
                User user = null;
                var offset = new DateTimeOffset(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(1).Day, DateTime.Now.Hour, 0, 0, default(TimeSpan));

                if (!_userSrv.CheckIfUserExist(username))
                {
                    user = new User
                    {
                        UserName = username,
                        Id = _userSrv.Insert(new User { UserName = username })
                    };

                    CacheManager.InsertItem("user", user, offset);
                }
                else
                {
                    user = _userSrv.GetByUsername(username);
                    CacheManager.InsertItem("user", user, offset);
                }

                ItemResponse<string> response = new ItemResponse<string>(username);
                HttpContext.Current.SetCookie("loggedIn", "true", DateTime.Now.AddDays(1));

                return Request.CreateResponse(HttpStatusCode.OK, response); //responseMessage;

            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region Bill Service Endpoints
        [Intercept("UserLogIn")]
        [Route("bill/income/all")]
        [HttpGet]
        public HttpResponseMessage GetAllIncome()
        {
            try
            {
                List<Income> result = null;
                result = _billSrv.GetAllIncome(CurrentUser.Id);
                ItemsResponse<Income> response = new ItemsResponse<Income>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("bill/expenses/all")]
        [HttpGet]
        public HttpResponseMessage GetAllExpenses()
        {
            try
            {


                List<Expenses> result = null;
                result = _billSrv.GetAllExpenses(CurrentUser.Id);
                ItemsResponse<Expenses> response = new ItemsResponse<Expenses>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("bill/income")]
        [HttpGet]
        public HttpResponseMessage GetIncome(int id = 0, string name = null, ScheduleTypes scheduleType = ScheduleTypes.Any, IncomeTypes incomeType = IncomeTypes.Any)
        {
            try
            {


                Income result = null;
                result = _billSrv.GetIncome(CurrentUser.Id, name);
                ItemResponse<Income> response = new ItemResponse<Income>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("bill/expenses")]
        [HttpGet]
        public HttpResponseMessage GetExpense(int id = 0, string name = null, ScheduleTypes scheduleType = ScheduleTypes.Any, ExpenseTypes billType = ExpenseTypes.Any)
        {
            try
            {


                Expenses result = null;
                result = _billSrv.GetExpense(CurrentUser.Id, name);
                ItemResponse<Expenses> response = new ItemResponse<Expenses>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("bill/income/chart")]
        [HttpGet]
        public HttpResponseMessage GetIncomeChart(DateTime? startDate = null, DateTime? endDate = null, ScheduleTypes chartSchedule = ScheduleTypes.Any)
        {
            try
            {


                Chart<List<float>> chart = null;
                chart = _billSrv.GetIncomeChart(CurrentUser.Id, ref chartSchedule, startDate, endDate);
                ItemResponse<Chart<List<float>>> response = new ItemResponse<Chart<List<float>>>(chart);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("bill/expenses/chart")]
        [HttpGet]
        public HttpResponseMessage GetExpensesChart(DateTime? startDate = null, DateTime? endDate = null, ScheduleTypes chartSchedule = ScheduleTypes.Any)
        {
            try
            {


                Chart<List<float>> result = null;
                result = _billSrv.GetExpensesChart(CurrentUser.Id, ref chartSchedule, startDate, endDate);
                ItemResponse<Chart<List<float>>> response = new ItemResponse<Chart<List<float>>>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("bill/combined/chart")]
        [HttpGet]
        public HttpResponseMessage GetCombinedChart(DateTime? startDate = null, DateTime? endDate = null, ScheduleTypes chartSchedule = ScheduleTypes.Any)
        {
            try
            {


                Chart<List<float>> result = null;
                result = _billSrv.GetCombinedChart(CurrentUser.Id, ref chartSchedule, startDate, endDate);
                ItemResponse<Chart<List<float>>> response = new ItemResponse<Chart<List<float>>>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("bill/income")]
        [HttpPost]
        public HttpResponseMessage InsertIncome(Income income)
        {
            try
            {
                BaseResponse response = null;


                if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    income.DateCreated = DateTime.Now;
                    income.DateModified = DateTime.Now;
                    _billSrv.InsertIncome(income);
                    response = new SuccessResponse();
                }

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("bill/expenses")]
        [HttpPost]
        public HttpResponseMessage InsertExpense(Expenses expense)
        {
            try
            {
                BaseResponse response = null;


                if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    expense.UserId = CurrentUser.Id;
                    expense.DateCreated = DateTime.Now;
                    expense.DateModified = DateTime.Now;
                    _billSrv.InsertExpense(expense);
                    response = new SuccessResponse();
                }

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("bill/income")]
        [HttpPut]
        public HttpResponseMessage UpdateIncome(Income income)
        {
            try
            {
                BaseResponse response = null;

                if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    income.UserId = CurrentUser.Id;
                    income.DateModified = DateTime.Now;
                    _billSrv.UpdateIncome(income);
                    response = new SuccessResponse();
                }

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("bill/expenses")]
        [HttpPut]
        public HttpResponseMessage UpdateExpense(Expenses expense)
        {
            try
            {
                BaseResponse response = null;

                if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    expense.UserId = CurrentUser.Id;
                    expense.DateModified = DateTime.Now;
                    _billSrv.UpdateExpense(expense);
                    response = new SuccessResponse();
                }

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("bill/income/{id:int}")]
        [HttpDelete]
        public HttpResponseMessage DeleteIncome(int id)
        {
            try
            {


                _billSrv.DeleteIncome(id);
                SuccessResponse response = new SuccessResponse();
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("bill/expenses/{id:int}")]
        [HttpDelete]
        public HttpResponseMessage DeleteExpense(int id)
        {
            try
            {


                _billSrv.DeleteExpense(id);
                SuccessResponse response = new SuccessResponse();
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region Card Service Endpoints
        [Intercept("UserLogIn")]
        [Route("cards/user")]
        [HttpGet]
        public HttpResponseMessage GetAllCardsByUser()
        {
            try
            {


                //List<StyledCard> list = _cardSrv.GetAll();
                List<StyledCard> filteredList = _cardSrv.Where(a => a.UserId == CurrentUser.Id).ToList();
                ItemsResponse<StyledCard> response = new ItemsResponse<StyledCard>(filteredList);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("cards/{id:int}")]
        [HttpGet]
        public HttpResponseMessage GetCard(int id)
        {
            try
            {


                StyledCard card = _cardSrv.Get(id);
                ItemResponse<StyledCard> response = new ItemResponse<StyledCard>(card);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("cards")]
        [HttpPost]
        public HttpResponseMessage InsertCard(StyledCard model)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    BaseResponse response = null;

                    model.UserId = CurrentUser.Id;
                    int id = (int)_cardSrv.Insert(model);
                    if (id == 0) { throw new Exception("Insert Failed"); }
                    response = new ItemResponse<int>(id);
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("cards")]
        [HttpPut]
        public HttpResponseMessage UpdateCard(StyledCard model)
        {
            try
            {
                BaseResponse response = null;

                if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    model.UserId = CurrentUser.Id;
                    _cardSrv.Update(model);
                    response = new SuccessResponse();
                }

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("cards/delete/{id:int}")]
        [HttpDelete]
        public HttpResponseMessage DeleteCard(int id)
        {
            try
            {


                _cardSrv.Delete(id);
                SuccessResponse response = new SuccessResponse();
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region Chart Service Endpoints
        [Intercept("UserLogIn")]
        [Route("charts/all")]
        [HttpGet]
        public HttpResponseMessage GetAllCharts()
        {
            try
            {


                List<Chart<object>> list = _chartsSrv.GetAll();
                ItemsResponse<Chart<object>> response = new ItemsResponse<Chart<object>>(list);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("charts/user/{username}")]
        [HttpGet]
        public HttpResponseMessage GetAllChartsByUser()
        {
            try
            {


                List<Chart<object>> list = _chartsSrv.GetAll();
                List<Chart<object>> filteredList = list?.Where(a => a.UserId == CurrentUser.Id).ToList();
                ItemsResponse<Chart<object>> response = new ItemsResponse<Chart<object>>(filteredList);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("charts/{id:int}")]
        [HttpGet]
        public HttpResponseMessage GetChart(int id)
        {
            try
            {


                Chart<object> chart = _chartsSrv.Get(id);
                ItemResponse<Chart<object>> response = new ItemResponse<Chart<object>>(chart);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("charts/int")]
        [HttpPost]
        public HttpResponseMessage InsertChart(ChartAddRequest<int> model)
        {
            try
            {
                BaseResponse response = null;

                if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    model.UserId = CurrentUser.Id;
                    int id = _chartsSrv.Insert(model);
                    if (id == 0) { throw new Exception("Insert Failed"); }
                    response = new ItemResponse<int>(id);
                }
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("charts/list/int")]
        [HttpPost]
        public HttpResponseMessage InsertChart(ChartAddRequest<List<int>> model)
        {
            try
            {
                BaseResponse response = null;

                if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    model.UserId = CurrentUser.Id;
                    int id = _chartsSrv.Insert(model);
                    if (id == 0) { throw new Exception("Insert Failed"); }
                    response = new ItemResponse<int>(id);
                }
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("charts/int")]
        [HttpPut]
        public HttpResponseMessage UpdateChart(ChartUpdateRequest<int> model)
        {
            try
            {
                BaseResponse response = null;

                if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    model.UserId = CurrentUser.Id;
                    _chartsSrv.Update(model);
                    response = new SuccessResponse();
                }
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("charts/list/int")]
        [HttpPut]
        public HttpResponseMessage UpdateChart(ChartUpdateRequest<List<int>> model)
        {
            try
            {
                BaseResponse response = null;

                if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    model.UserId = CurrentUser.Id;
                    _chartsSrv.Update(model);
                    response = new SuccessResponse();
                }
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("charts/delete/{id:int}")]
        [HttpDelete]
        public HttpResponseMessage DeleteChart(int id)
        {
            try
            {

                BaseResponse response = null;


                _chartsSrv.Delete(id);
                response = new SuccessResponse();
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region Other Endpoints
        [Route("send/email")]
        [HttpPost]
        public async Task<HttpResponseMessage> SendEmail(Dictionary<string, string> emailRequest)
        {
            try
            {
                if (!emailRequest.ContainsKey("email") || !emailRequest.ContainsKey("name") || !emailRequest.ContainsKey("subject") || !emailRequest.ContainsKey("messageText"))
                { throw new Exception("Invalid Model"); }
                if (!emailRequest.ContainsKey("toEmail")) { emailRequest["toEmail"] = "nileoverstreet@gmail.com"; }
                if (!emailRequest.ContainsKey("messageHtml"))
                {
                    string phoneNumber = (emailRequest.ContainsKey("phone") ? "Phone Number: " + emailRequest["phone"] : string.Empty);
                    emailRequest["messageHtml"] = "<div> Email From Contact Me Page </div>"
                                                + "<div>" + "Name: " + emailRequest["name"] + "</div>"
                                                + "<div>" + "Email: " + emailRequest["email"] + "</div>"
                                                + "<div>" + phoneNumber + "</div>"
                                                + "<div>" + "Message: " + emailRequest["messageText"] + "</div>";
                }
                if (!await _sendGridSrv.Send(emailRequest["email"], emailRequest["name"], emailRequest["toEmail"], emailRequest["subject"], emailRequest["messageText"], emailRequest["messageHtml"]))
                { throw new Exception("Email Was Not Sent"); }
                SuccessResponse response = new SuccessResponse();
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route("view/code/{fileName}")]
        [HttpGet]
        public HttpResponseMessage GetCode(string fileName)
        {
            try
            {
                BaseResponse response = null;
                string code = null;
                switch (fileName)
                {
                    case "dymanicGraphsController":
                        code = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Scripts/app/controllers/dynamicGraphsController.js"));
                        response = new ItemResponse<string>(code);
                        break;

                    case "cardController":
                        string[] lines = File.ReadAllLines(HttpContext.Current.Server.MapPath("~/Scripts/app/controllers/generalControllers.js"));
                        code = GetStringWithinLines(110, 411, lines);
                        response = new ItemResponse<string>(code);
                        break;

                    case "dymanicGraphsDirective":
                        code = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Scripts/app/directives/renderGraph.js"));
                        response = new ItemResponse<string>(code);
                        break;
                }
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {

                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route("config/enums/{enumType}")]
        [HttpGet]
        public HttpResponseMessage GetEnums(string enumType)
        {
            try
            {
                string[] enumTypes = enumType.Split(',');
                ItemsResponse<string, Dictionary<int, string>> response = null;
                Dictionary<string, Dictionary<int, string>> result = new Dictionary<string, Dictionary<int, string>>();

                if (enumTypes.FirstOrDefault(a => a == "income") != null)
                {
                    result.Add("income", typeof(IncomeTypes).ToDictionary<IncomeTypes>());
                }

                if (enumTypes.FirstOrDefault(a => a == "expense") != null)
                {
                    result.Add("expense", typeof(ExpenseTypes).ToDictionary<ExpenseTypes>());
                }

                if (enumTypes.FirstOrDefault(a => a == "schedule") != null)
                {
                    result.Add("schedule", typeof(ScheduleTypes).ToDictionary<ScheduleTypes>());
                }

                if (enumTypes.FirstOrDefault(a => a == "chart") != null)
                {
                    result.Add("chart", typeof(ChartTypes).ToDictionary<ChartTypes>());
                }

                response = new ItemsResponse<string, Dictionary<int, string>>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {

                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("config/site")]
        [HttpGet]
        public HttpResponseMessage GetSite(string url)
        {
            try
            {
                BaseResponse response = null;
                if (url.HitEndpoint() != null)
                    response = new SuccessResponse();
                else
                    response = new ErrorResponse("Could not get a response to the site " + url);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
            catch (Exception ex)
            {

                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }


        #endregion

    }
}
