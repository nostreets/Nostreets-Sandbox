using NostreetsORM.Interfaces;
using NostreetsRouter.Models.Responses;
using Nostreets_Services.Domain;
using Nostreets_Services.Domain.Bills;
using Nostreets_Services.Domain.Cards;
using Nostreets_Services.Domain.Charts;
using Nostreets_Services.Enums;
using Nostreets_Services.Interfaces.Services;
using Nostreets_Services.Models.Request;
using Nostreets_Services.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Net.Http.Headers;

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

        public SandoxApiController(IChartsExtended chartsInject, ISendGridService sendGridInject, IDBService<StyledCard> cardInject, IUserService userInject, IBillService billsInject)
        {
            _chartsSrv = chartsInject;
            _sendGridSrv = sendGridInject;
            _cardSrv = cardInject;
            _userSrv = userInject;
            _billSrv = billsInject;

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

        private User GetCurrentUser()
        {
            if (Request.GetCookie("loggedIn") == null) { return null; }
            else
            {
                string uid = CacheManager.GetItem<string>("uid");
                if (uid == null) { return null; }

                User user = _userSrv.Get(uid);
                if (user == null) { return null; }

                return user;
            }
        }

        private bool IsLoggedIn { get { return GetCurrentUser() != null ? true : false; } }
        #endregion

        #region User Service Endpoints
        [Route("user")]
        [HttpGet]
        public HttpResponseMessage LogInUser(string username)
        {
            try
            {
                HttpResponseMessage responseMessage = null;
                var offset = new DateTimeOffset(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(1).Day, DateTime.Now.Hour, 0, 0, default(TimeSpan));

                if (!_userSrv.CheckIfUserExist(username))
                {
                    string id = _userSrv.Insert(new User { UserName = username });
                    CacheManager.InsertItem("uid", id, offset);
                }
                else
                {
                    User user = _userSrv.GetByUsername(username);
                    CacheManager.InsertItem("uid", user.Id, offset);
                }

                ItemResponse<string> response = new ItemResponse<string>(username);
                responseMessage = Request.CreateResponse(HttpStatusCode.OK, response);
                Request.SetCookie(ref responseMessage, "loggedIn", "true", offset);


                return responseMessage;

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
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }

                List<Income> result = null;
                result = _billSrv.GetAllIncome(GetCurrentUser().Id);
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
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }

                List<Expenses> result = null;
                result = _billSrv.GetAllExpenses(GetCurrentUser().Id);
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
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }

                Income result = null;
                result = _billSrv.GetIncome(GetCurrentUser().Id, name);
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
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }

                Expenses result = null;
                result = _billSrv.GetExpense(GetCurrentUser().Id, name);
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
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }

                Chart<List<float>> result = null;
                result = _billSrv.GetIncomeChart(GetCurrentUser().Id, startDate, endDate);
                ItemResponse<Chart<List<float>>> response = new ItemResponse<Chart<List<float>>>(result);
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
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }

                Chart<List<float>> result = null;
                result = _billSrv.GetExpensesChart(GetCurrentUser().Id, startDate, endDate);
                ItemResponse<Chart<List<float>>> response = new ItemResponse<Chart<List<float>>>(result);
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
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }

                Chart<List<float>> result = null;
                result = _billSrv.GetCombinedChart(GetCurrentUser().Id, startDate, endDate);
                ItemResponse<Chart<List<float>>> response = new ItemResponse<Chart<List<float>>>(result);
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

                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }
                else if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    income.UserId = GetCurrentUser().Id;
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

        [Route("bill/expenses")]
        [HttpPost]
        public HttpResponseMessage InsertExpense(Expenses expense)
        {
            try
            {
                BaseResponse response = null;

                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }
                else if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    expense.UserId = GetCurrentUser().Id;
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

        [Route("bill/income")]
        [HttpPut]
        public HttpResponseMessage UpdateIncome(Income income)
        {
            try
            {
                BaseResponse response = null;
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }
                else if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    income.UserId = GetCurrentUser().Id;
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

        [Route("bill/expenses")]
        [HttpPut]
        public HttpResponseMessage UpdateExpense(Expenses expense)
        {
            try
            {
                BaseResponse response = null;
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }
                else if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    expense.UserId = GetCurrentUser().Id;
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

        [Route("bill/income/{id:int}")]
        [HttpDelete]
        public HttpResponseMessage DeleteIncome(int id)
        {
            try
            {
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }

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

        [Route("bill/expenses/{id:int}")]
        [HttpDelete]
        public HttpResponseMessage DeleteExpense(int id)
        {
            try
            {
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }

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
        [Route("cards/user")]
        [HttpGet]
        public HttpResponseMessage GetAllCardsByUser()
        {
            try
            {
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }

                List<StyledCard> list = _cardSrv.GetAll();
                List<StyledCard> filteredList = list != null ? list.Where(a => a.UserId == GetCurrentUser().Id).ToList() : null;
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
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }

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
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }
                else if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    BaseResponse response = null;

                    model.UserId = GetCurrentUser().Id;
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

        [Route("cards")]
        [HttpPut]
        public HttpResponseMessage UpdateCard(StyledCard model, string username)
        {
            try
            {
                BaseResponse response = null;
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }
                else if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    model.UserId = GetCurrentUser().Id;
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

        [Route("cards/delete/{id:int}")]
        [HttpDelete]
        public HttpResponseMessage DeleteCard(int id)
        {
            try
            {
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }

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
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }

                List<Chart<object>> list = _chartsSrv.GetAll();
                ItemsResponse<Chart<object>>  response = new ItemsResponse<Chart<object>>(list);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route("charts/user")]
        [HttpGet]
        public HttpResponseMessage GetAllChartsByUser()
        {
            try
            {
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }

                List<Chart<object>> list = _chartsSrv.GetAll();
                List<Chart<object>> filteredList = list != null ? list.Where(a => a.UserId == GetCurrentUser().Id).ToList() : null;
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
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }

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
                BaseResponse response = null;
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }
                else if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    model.UserId = GetCurrentUser().Id;
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

        [Route("charts/list/int")]
        [HttpPost]
        public HttpResponseMessage InsertChart(ChartAddRequest<List<int>> model, string username)
        {
            try
            {
                BaseResponse response = null;
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }
                else if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    model.UserId = GetCurrentUser().Id;
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

        [Route("charts/int")]
        [HttpPut]
        public HttpResponseMessage UpdateChart(ChartUpdateRequest<int> model, string username)
        {
            try
            {
                BaseResponse response = null;
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }
                else if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    model.UserId = GetCurrentUser().Id;
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

        [Route("charts/list/int")]
        [HttpPut]
        public HttpResponseMessage UpdateChart(ChartUpdateRequest<List<int>> model, string username)
        {
            try
            {
                BaseResponse response = null;
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }
                else if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    model.UserId = GetCurrentUser().Id;
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

        [Route("charts/delete/{id:int}")]
        [HttpDelete]
        public HttpResponseMessage DeleteChart(int id)
        {
            try
            {

                BaseResponse response = null;
                if (!IsLoggedIn) { throw new Exception("User is not logged in..."); }
                else
                {
                    _chartsSrv.Delete(id);
                    response = new SuccessResponse();
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
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
        #endregion

    }
}
