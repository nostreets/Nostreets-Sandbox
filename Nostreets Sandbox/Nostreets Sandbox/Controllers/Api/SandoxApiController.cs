using Nostreets_Sandbox.App_Start;
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

namespace Nostreets_Sandbox.Controllers.Api
{
    [RoutePrefix("api")]
    public class SandoxApiController : ApiController
    {
        public SandoxApiController()
        {
            _chartsService = _chartsService.WindsorResolve(WindsorConfig.GetContainer());
            _emailService = _emailService.WindsorResolve(WindsorConfig.GetContainer());
            _cardService = _cardService.WindsorResolve(WindsorConfig.GetContainer());
            _userService = _userService.WindsorResolve(WindsorConfig.GetContainer());
            _billService = _billService.WindsorResolve(WindsorConfig.GetContainer());
        }

        IChartService _chartsService = null;
        IEmailService _emailService = null;
        IDBService<StyledCard> _cardService = null;
        IUserService _userService = null;
        IBillService _billService = null;

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

        private User CurrentUser { get { return _userService.SessionUser; } }


        #endregion

        #region User Service Endpoints
        [HttpGet, Route("user")]
        public HttpResponseMessage LogInUser(string username, string password)
        {
            try
            {

                _userService.LogIn(username, password);

                ItemResponse<string> response = new ItemResponse<string>(username);
                HttpContext.Current.SetCookie("loggedIn", "true");

                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpPost, Route("register"), Intercept("Session", "PostMapRequestHandler")]
        public async Task<HttpResponseMessage> RegisterAsync(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string id = await _userService.RegisterAsync(user);
                    ItemResponse<string> response = new ItemResponse<string>(id);
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                    throw new Exception("user is invalid...");

            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpGet, Route("logout")]
        public HttpResponseMessage LogOutUser()
        {
            try
            {

                _userService.LogOut();
                SuccessResponse response = new SuccessResponse();
                HttpContext.Current.SetCookie("loggedIn", "false");
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpGet, Route("checkUsername")]
        public HttpResponseMessage CheckUsername(string username)
        {
            try
            {
                bool result = _userService.CheckIfUsernameExist(username);
                ItemResponse<bool> response = new ItemResponse<bool>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpGet, Route("checkEmail")]
        public HttpResponseMessage CheckEmail(string email)
        {
            try
            {
                bool result = _userService.CheckIfEmailExist(email);
                ItemResponse<bool> response = new ItemResponse<bool>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region Bill Service Endpoints
        [HttpGet, Route("bill/income/all"), Intercept("UserLogIn")]
        public HttpResponseMessage GetAllIncome()
        {
            try
            {
                List<Income> result = null;
                result = _billService.GetAllIncome(CurrentUser.Id);
                ItemsResponse<Income> response = new ItemsResponse<Income>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
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


                List<Expense> result = null;
                result = _billService.GetAllExpenses(CurrentUser.Id);
                ItemsResponse<Expense> response = new ItemsResponse<Expense>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("bill/income")]
        [HttpGet]
        public HttpResponseMessage GetIncome(int id = 0, string name = null, ScheduleTypes scheduleType = ScheduleTypes.Any, IncomeType incomeType = IncomeType.Any)
        {
            try
            {


                Income result = null;
                result = _billService.GetIncome(CurrentUser.Id, name);
                ItemResponse<Income> response = new ItemResponse<Income>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("bill/expenses")]
        [HttpGet]
        public HttpResponseMessage GetExpense(int id = 0, string name = null, ScheduleTypes scheduleType = ScheduleTypes.Any, ExpenseType billType = ExpenseType.Any)
        {
            try
            {


                Expense result = null;
                result = _billService.GetExpense(CurrentUser.Id, name);
                ItemResponse<Expense> response = new ItemResponse<Expense>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
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
                chart = _billService.GetIncomeChart(CurrentUser.Id, out chartSchedule, startDate, endDate);
                ItemResponse<Chart<List<float>>> response = new ItemResponse<Chart<List<float>>>(chart);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
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
                result = _billService.GetExpensesChart(CurrentUser.Id, out chartSchedule, startDate, endDate);
                ItemResponse<Chart<List<float>>> response = new ItemResponse<Chart<List<float>>>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
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
                result = _billService.GetCombinedChart(CurrentUser.Id, out chartSchedule, startDate, endDate);
                ItemResponse<Chart<List<float>>> response = new ItemResponse<Chart<List<float>>>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
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
                    _billService.InsertIncome(income);
                    response = new SuccessResponse();
                }

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("bill/expenses")]
        [HttpPost]
        public HttpResponseMessage InsertExpense(Expense expense)
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
                    _billService.InsertExpense(expense);
                    response = new SuccessResponse();
                }

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
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
                    _billService.UpdateIncome(income);
                    response = new SuccessResponse();
                }

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("bill/expenses")]
        [HttpPut]
        public HttpResponseMessage UpdateExpense(Expense expense)
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
                    _billService.UpdateExpense(expense);
                    response = new SuccessResponse();
                }

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
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


                _billService.DeleteIncome(id);
                SuccessResponse response = new SuccessResponse();
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
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


                _billService.DeleteExpense(id);
                SuccessResponse response = new SuccessResponse();
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
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
                List<StyledCard> filteredList = _cardService.Where(a => a.UserId == CurrentUser.Id)?.ToList();
                ItemsResponse<StyledCard> response = new ItemsResponse<StyledCard>(filteredList);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
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


                StyledCard card = _cardService.Get(id);
                ItemResponse<StyledCard> response = new ItemResponse<StyledCard>(card);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
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
                    int id = (int)_cardService.Insert(model);
                    if (id == 0) { throw new Exception("Insert Failed"); }
                    response = new ItemResponse<int>(id);
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
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
                    _cardService.Update(model);
                    response = new SuccessResponse();
                }

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
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


                _cardService.Delete(id);
                SuccessResponse response = new SuccessResponse();
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
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


                List<Chart<object>> list = _chartsService.GetAll();
                ItemsResponse<Chart<object>> response = new ItemsResponse<Chart<object>>(list);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("charts/user")]
        [HttpGet]
        public HttpResponseMessage GetAllChartsByUser()
        {
            try
            {


                List<Chart<object>> list = _chartsService.GetAll();
                List<Chart<object>> filteredList = list?.Where(a => a.UserId == CurrentUser.Id).ToList();
                ItemsResponse<Chart<object>> response = new ItemsResponse<Chart<object>>(filteredList);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
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


                Chart<object> chart = _chartsService.Get(id);
                ItemResponse<Chart<object>> response = new ItemResponse<Chart<object>>(chart);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
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
                    int id = _chartsService.Insert(model, (a) =>
                        {
                            return new Chart<int>
                            {
                                Labels = a.Labels,
                                Legend = a.Legend,
                                Name = a.Name,
                                Series = a.Series,
                                TypeId = a.TypeId,
                                UserId = a.UserId,
                                DateModified = DateTime.Now,
                                DateCreated = DateTime.Now
                            };
                        });
                    if (id == 0) { throw new Exception("Insert Failed"); }
                    response = new ItemResponse<int>(id);
                }
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("charts/list/int")]
        [HttpPost]
        public HttpResponseMessage InsertChart(ChartAddRequest model)
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

                    int id = _chartsService.Insert(model,
                        (a) =>
                        {
                            return new Chart<List<int>>
                            {
                                Labels = a.Labels,
                                Legend = a.Legend,
                                Name = a.Name,
                                Series = a.Series,
                                TypeId = a.TypeId,
                                UserId = a.UserId,
                                DateModified = DateTime.Now,
                                DateCreated = DateTime.Now
                            };
                        });

                    if (id == 0) { throw new Exception("Insert Failed"); }
                    response = new ItemResponse<int>(id);
                }
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
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
                    _chartsService.Update(model, (a) =>
                        {
                            return new Chart<int>
                            {
                                Labels = a.Labels,
                                Legend = a.Legend,
                                Name = a.Name,
                                Series = a.Series,
                                TypeId = a.TypeId,
                                UserId = a.UserId,
                                DateModified = DateTime.Now
                            };
                        });
                    response = new SuccessResponse();
                }
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Intercept("UserLogIn")]
        [Route("charts/list/int")]
        [HttpPut]
        public HttpResponseMessage UpdateChart(ChartUpdateRequest model)
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
                    _chartsService.Update(model, (a) =>
                        {
                            return new Chart<List<int>>
                            {
                                Labels = a.Labels,
                                Legend = a.Legend,
                                Name = a.Name,
                                Series = a.Series,
                                TypeId = a.TypeId,
                                UserId = a.UserId,
                                DateModified = DateTime.Now
                            };
                        });
                    response = new SuccessResponse();
                }
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
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


                _chartsService.Delete(id);
                response = new SuccessResponse();
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
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
                if (!emailRequest.ContainsKey("fromEmail") || !emailRequest.ContainsKey("name") || !emailRequest.ContainsKey("subject") || !emailRequest.ContainsKey("messageText"))
                { throw new Exception("Invalid Model"); }
                if (!emailRequest.ContainsKey("toEmail")) { emailRequest["toEmail"] = "nileoverstreet@gmail.com"; }
                if (!emailRequest.ContainsKey("messageHtml"))
                {
                    string phoneNumber = (emailRequest.ContainsKey("phone") ? "Phone Number: " + emailRequest["phone"] : string.Empty);
                    emailRequest["messageHtml"] = "<div> Email From Contact Me Page </div>"
                                                + "<div>" + "Name: " + emailRequest["name"] + "</div>"
                                                + "<div>" + "Email: " + emailRequest["fromEmail"] + "</div>"
                                                + "<div>" + phoneNumber + "</div>"
                                                + "<div>" + "Message: " + emailRequest["messageText"] + "</div>";
                }
                if (!await _emailService.SendAsync(emailRequest["fromEmail"], emailRequest["toEmail"], emailRequest["subject"], emailRequest["messageText"], emailRequest["messageHtml"]))
                { throw new Exception("Email Was Not Sent"); }
                SuccessResponse response = new SuccessResponse();
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex);
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

                ErrorResponse response = new ErrorResponse(ex);
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
                    var types = typeof(IncomeType).ToDictionary<IncomeType>();
                    for (int i = 0; i < types.Count; i++)
                    {
                        types[i] = types[i].SafeName();
                    }
                    result.Add("income", types);
                }

                if (enumTypes.FirstOrDefault(a => a == "expense") != null)
                {
                    var types = typeof(ExpenseType).ToDictionary<ExpenseType>();
                    for (int i = 0; i < types.Count; i++)
                    {
                        types[i] = types[i].SafeName();
                    }
                    result.Add("expense", types);
                }

                if (enumTypes.FirstOrDefault(a => a == "schedule") != null)
                {
                    var types = typeof(ScheduleTypes).ToDictionary<ScheduleTypes>();
                    for (int i = 0; i < types.Count; i++)
                    {
                        types[i] = types[i].SafeName();
                    }
                    result.Add("schedule", types);
                }

                if (enumTypes.FirstOrDefault(a => a == "chart") != null)
                {
                    var types = typeof(ChartType).ToDictionary<ChartType>();
                    for (int i = 0; i < types.Count; i++)
                    {
                        types[i] = types[i].SafeName();
                    }
                    result.Add("chart", types);
                }

                response = new ItemsResponse<string, Dictionary<int, string>>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {

                ErrorResponse response = new ErrorResponse(ex);
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

                ErrorResponse response = new ErrorResponse(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }


        #endregion

    }
}
