using Microsoft.Practices.Unity;
using Nostreets_Sandbox.App_Start;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using Nostreets_Services.Interfaces.Services;
using Nostreets_Services.Domain.Charts;
using Nostreets_Services.Models.Request;
using Nostreets_Services.Services.Database;
using Nostreets_Services.Services.Web;
using Nostreets_Services.Domain.Cards;
using Nostreets_Services.Domain;
using System.Linq;
using NostreetsORM;
using NostreetsORM.Interfaces;
using Nostreets_Services.Utilities;
using Nostreets_Services.Domain.Bills;
using Nostreets_Services.Enums;
using NostreetsRouter.Models.Responses;

namespace Nostreets_Sandbox.Controllers.Api
{
    [RoutePrefix("api")]
    public class SandoxApiController : ApiController
    {
        IChartsExtended _chartsSrv = null;
        ISendGridService _sendGridSrv = null;
        IDBService<StyledCard> _cardSrv = null;
        IUserService _userSrv = null;
        IBillService _billSrv = null;

        public SandoxApiController(/*IDBService<Chart, int, ChartAddRequest, ChartUpdateRequest> chartsInject, ISendGridService sendGridInject*/)
        {
            _chartsSrv = UnityConfig.GetContainer().Resolve<ChartsService>();
            _sendGridSrv = UnityConfig.GetContainer().Resolve<SendGridService>();
            _cardSrv = UnityConfig.GetContainer().Resolve<DBService<StyledCard>>();
            _userSrv = UnityConfig.GetContainer().Resolve<UserService>();
            _billSrv = UnityConfig.GetContainer().Resolve<BillService>();

        }

        private string GetStringWithinLines(int begining, int ending, string[] file)
        {

            string result = null;
            for (int i = begining - 1; i <= ending; i++)
            {
                result += "\r\n" + file[i];
            }
            return result;
        }

        #region User Service Endpoints
        [Route("user/{username}")]
        [HttpGet]
        public HttpResponseMessage LogInUser(string username)
        {
            try
            {
                if (!_userSrv.CheckIfUserExist(username))
                {
                    string id = _userSrv.Insert(new User { UserName = username });
                    var offset = new DateTimeOffset(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(1).Day, DateTime.Now.Hour, 0, 0, default(TimeSpan));
                    CacheManager.InsertItem("uid", id, offset);
                }
                else
                {
                    User user = _userSrv.GetByUsername(username);
                    var offset = new DateTimeOffset(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(1).Day, DateTime.Now.Hour, 0, 0, default(TimeSpan));
                    CacheManager.InsertItem("uid", user.Id, offset);
                }


                ItemResponse<string> response = new ItemResponse<string>(username);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region Bill Service Endpoints
        [Route("bill/income/all")]
        [HttpGet]
        public HttpResponseMessage GetAllIncome()
        {
            try
            {
                List<Income> result = null;

                if (!CacheManager.Contains("uid")) { throw new Exception("User is not logged in"); }
                else
                {
                    User user = _userSrv.Get(CacheManager.GetItem<string>("uid"));
                    result = _billSrv.GetAllIncome(user.Id);
                }

                ItemsResponse<Income> response = new ItemsResponse<Income>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route("bill/expenses/all")]
        [HttpGet]
        public HttpResponseMessage GetAllExpenses()
        {
            try
            {
                List<Expenses> result = null;

                if (!CacheManager.Contains("uid")) { throw new Exception("User is not logged in"); }
                else
                {
                    User user = _userSrv.Get(CacheManager.GetItem<string>("uid"));
                    result = _billSrv.GetAllExpenses(user.Id);
                }

                ItemsResponse<Expenses> response = new ItemsResponse<Expenses>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route("bill/income")]
        [HttpGet]
        public HttpResponseMessage GetIncome(int id = 0, string name = null, ScheduleTypes scheduleType = ScheduleTypes.Any, IncomeTypes incomeType = IncomeTypes.Any)
        {
            try
            {
                Income result = null;

                if (!CacheManager.Contains("uid")) { throw new Exception("User is not logged in"); }
                else
                {
                    User user = _userSrv.Get(CacheManager.GetItem<string>("uid"));
                    result = _billSrv.GetIncome(user.Id, name);
                }

                ItemResponse<Income> response = new ItemResponse<Income>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route("bill/expenses")]
        [HttpGet]
        public HttpResponseMessage GetExpense(int id = 0, string name = null, ScheduleTypes scheduleType = ScheduleTypes.Any, ExpenseTypes billType = ExpenseTypes.Any)
        {
            try
            {
                Expenses result = null;

                if (!CacheManager.Contains("uid")) { throw new Exception("User is not logged in"); }
                else
                {
                    User user = _userSrv.Get(CacheManager.GetItem<string>("uid"));
                    result = _billSrv.GetExpense(user.Id, name);
                }

                ItemResponse<Expenses> response = new ItemResponse<Expenses>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route("bill/income/chart")]
        [HttpGet]
        public HttpResponseMessage GetIncomeChart(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                Chart<List<decimal>> result = null;

                if (!CacheManager.Contains("uid")) { throw new Exception("User is not logged in"); }
                else
                {
                    User user = _userSrv.Get(CacheManager.GetItem<string>("uid"));
                    result = _billSrv.GetIncomeChart(user.Id);
                }

                ItemResponse<Chart<List<decimal>>> response = new ItemResponse<Chart<List<decimal>>>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route("bill/expenses/chart")]
        [HttpGet]
        public HttpResponseMessage GetExpensesChart(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                Chart<List<decimal>> result = null;

                if (!CacheManager.Contains("uid")) { throw new Exception("User is not logged in"); }
                else
                {
                    User user = _userSrv.Get(CacheManager.GetItem<string>("uid"));
                    result = _billSrv.GetExpensesChart(user.Id);
                }

                ItemResponse<Chart<List<decimal>>> response = new ItemResponse<Chart<List<decimal>>>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route("bill/combined/chart")]
        [HttpGet]
        public HttpResponseMessage GetCombinedChart(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                Chart<List<decimal>> result = null;

                if (!CacheManager.Contains("uid")) { throw new Exception("User is not logged in"); }
                else
                {
                    User user = _userSrv.Get(CacheManager.GetItem<string>("uid"));
                    result = _billSrv.GetCombinedChart(user.Id);
                }

                ItemResponse<Chart<List<decimal>>> response = new ItemResponse<Chart<List<decimal>>>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route("bill/income")]
        [HttpPost]
        public HttpResponseMessage InsertIncome(Income income)
        {
            try
            {
                BaseResponse response = null;

                if (!CacheManager.Contains("uid"))
                {
                    throw new Exception("User is not logged in");
                }
                else if (ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    User user = _userSrv.Get(CacheManager.GetItem<string>("uid"));
                    income.UserId = user.Id;
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

        [Route("bill/expenses")]
        [HttpPost]
        public HttpResponseMessage InsertExpense(Expenses expense)
        {
            try
            {
                BaseResponse response = null;

                if (!CacheManager.Contains("uid"))
                {
                    throw new Exception("User is not logged in");
                }
                else if (ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    User user = _userSrv.Get(CacheManager.GetItem<string>("uid"));
                    expense.UserId = user.Id;
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

        [Route("bill/income")]
        [HttpPut]
        public HttpResponseMessage UpdateIncome(Income income)
        {
            try
            {
                BaseResponse response = null;

                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
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

        [Route("bill/expenses")]
        [HttpPut]
        public HttpResponseMessage UpdateExpense(Expenses expense)
        {
            try
            {
                BaseResponse response = null;
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
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

        [Route("bill/income/{id:int}")]
        [HttpDelete]
        public HttpResponseMessage DeleteIncome(int id)
        {
            try
            {
                _billSrv.DeleteIncome(id);
                SuccessResponse response = new SuccessResponse();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route("bill/expenses/{id:int}")]
        [HttpDelete]
        public HttpResponseMessage DeleteExpense(int id)
        {
            try
            {

                _billSrv.DeleteExpense(id);
                SuccessResponse response = new SuccessResponse();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region Card Service Endpoints
        [Route("cards/all")]
        [HttpGet]
        public HttpResponseMessage GetAllCards()
        {
            try
            {
                List<StyledCard> list = _cardSrv.GetAll();
                ItemsResponse<StyledCard> response = new ItemsResponse<StyledCard>(list);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route("cards/user/{username}")]
        [HttpGet]
        public HttpResponseMessage GetAllCardsByUser(string username)
        {
            try
            {
                User user = _userSrv.GetByUsername(username);
                List<StyledCard> list = _cardSrv.GetAll();
                List<StyledCard> filteredList = list != null ? list.Where(a => a.UserId == user.Id).ToList() : null;
                ItemsResponse<StyledCard> response = new ItemsResponse<StyledCard>(filteredList);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

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

        [Route("cards")]
        [HttpPost]
        public HttpResponseMessage InsertCard(StyledCard model, string username)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.UserId = _userSrv.GetByUsername(username).Id;
                    int id = (int)_cardSrv.Insert(model);
                    if (id == 0) { throw new Exception("Insert Failed"); }
                    ItemResponse<int> response = new ItemResponse<int>(id);
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route("cards")]
        [HttpPut]
        public HttpResponseMessage UpdateCard(StyledCard model, string username)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.UserId = _userSrv.GetByUsername(username).Id;
                    _cardSrv.Update(model);
                    SuccessResponse response = new SuccessResponse();
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route("cards/delete/{id:int}")]
        [HttpDelete]
        public HttpResponseMessage DeleteCard(int id)
        {
            try
            {
                _chartsSrv.Delete(id);
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

        [Route("charts/user/{username}")]
        [HttpGet]
        public HttpResponseMessage GetAllChartsByUser(string username)
        {
            try
            {
                User user = _userSrv.GetByUsername(username);
                List<Chart<object>> list = _chartsSrv.GetAll();
                List<Chart<object>> filteredList = list != null ? list.Where(a => a.UserId == user.Id).ToList() : null;
                ItemsResponse<Chart<object>> response = new ItemsResponse<Chart<object>>(filteredList);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

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

        [Route("charts/int")]
        [HttpPost]
        public HttpResponseMessage InsertChart(ChartAddRequest<int> model, string username)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.UserId = _userSrv.GetByUsername(username).Id;
                    int id = _chartsSrv.Insert(model);
                    if (id == 0) { throw new Exception("Insert Failed"); }
                    ItemResponse<int> response = new ItemResponse<int>(id);
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route("charts/list/int")]
        [HttpPost]
        public HttpResponseMessage InsertChart(ChartAddRequest<List<int>> model, string username)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.UserId = _userSrv.GetByUsername(username).Id;
                    int id = _chartsSrv.Insert(model);
                    if (id == 0) { throw new Exception("Insert Failed"); }
                    ItemResponse<int> response = new ItemResponse<int>(id);
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route("charts/int")]
        [HttpPut]
        public HttpResponseMessage UpdateChart(ChartUpdateRequest<int> model, string username)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.UserId = _userSrv.GetByUsername(username).Id;
                    _chartsSrv.Update(model);
                    SuccessResponse response = new SuccessResponse();
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route("charts/list/int")]
        [HttpPut]
        public HttpResponseMessage UpdateChart(ChartUpdateRequest<List<int>> model, string username)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.UserId = _userSrv.GetByUsername(username).Id;
                    _chartsSrv.Update(model);
                    SuccessResponse response = new SuccessResponse();
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route("charts/delete/{id:int}")]
        [HttpDelete]
        public HttpResponseMessage DeleteChart(int id)
        {
            try
            {
                _chartsSrv.Delete(id);
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
                    string phoneNumber = (emailRequest.ContainsKey("phone") ? "Phone Number: " + emailRequest["phone"] : "");
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

        #endregion

    }
}
