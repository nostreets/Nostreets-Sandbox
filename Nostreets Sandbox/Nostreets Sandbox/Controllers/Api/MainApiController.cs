using Nostreets_Sandbox.App_Start;

using Nostreets_Services.Classes.Domain.Bills;
using Nostreets_Services.Classes.Domain.Cards;
using Nostreets_Services.Classes.Domain.Charts;
using Nostreets_Services.Classes.Domain.Product;
using Nostreets_Services.Classes.Domain.Users;
using Nostreets_Services.Classes.Domain.Web;
using Nostreets_Services.Enums;
using Nostreets_Services.Interfaces.Services;
using Nostreets_Services.Models.Request;

using NostreetsExtensions.Extend.IOC;
using NostreetsExtensions.Extend.Web;
using NostreetsExtensions.Interfaces;

using NostreetsInterceptor;

using NostreetsRouter.Models.Responses;
using OBL_Website.Controllers.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Nostreets_Sandbox.Controllers.Api
{
    [RoutePrefix("api")]
    public class MainApiController : System.Web.Http.ApiController
    {
        public MainApiController()
        {
            var container = WindsorConfig.GetContainer();
            _chartsSrv = _chartsSrv.WindsorResolve(container);
            _emailSrv = _emailSrv.WindsorResolve(container);
            _cardSrv = _cardSrv.WindsorResolve(container);
            _userSrv = _userSrv.WindsorResolve(container);
            _billSrv = _billSrv.WindsorResolve(container);
            _errorLog = _errorLog.WindsorResolve(container);
            _delevopProductSrv = _delevopProductSrv.WindsorResolve(container);

            _oblEndpoints = new Endpoints(_emailSrv, _userSrv);
        }

        private IBillService _billSrv = null;
        private IChartService _chartsSrv = null;
        private IEmailService _emailSrv = null;
        private IUserService _userSrv = null;
        private IDBService<StyledCard, int> _cardSrv = null;
        private IDBService<WebRequestError, int> _errorLog = null;
        private IDBService<ProductDevelopment, int> _delevopProductSrv = null;
        private Endpoints _oblEndpoints = null;

        #region Private Members

        private HttpResponseMessage ErrorResponse(Exception ex)
        {
            _errorLog.Insert(new WebRequestError(Request, ex)
            {
                UserId = _userSrv.SessionUser?.Id
            });
            return Request.CreateResponse(HttpStatusCode.InternalServerError, new ErrorResponse(ex));
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

        #endregion Private Members

        #region User Service Endpoints

        [HttpGet, Route("checkEmail")]
        public HttpResponseMessage CheckIfEmailExist(string email)
        {
            try
            {
                ItemResponse<bool> response = _oblEndpoints.CheckIfEmailExist(email);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [HttpGet, Route("checkUsername")]
        public HttpResponseMessage CheckIfUsernameExist(string username)
        {
            try
            {
                ItemResponse<bool> response = _oblEndpoints.CheckIfUsernameExist(username);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [HttpGet, Route("user/forgotPassword")]
        public HttpResponseMessage ForgotPasswordEmail(string username)
        {
            try
            {
                SuccessResponse response = _oblEndpoints.ForgotPasswordEmail(username);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [HttpGet, Route("user/session"), Intercept("LoggedIn")]
        public HttpResponseMessage GetSessionUser()
        {
            try
            {
                ItemResponse<User> response = _oblEndpoints.GetSessionUser();
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [HttpPost, Route("login")]
        public async Task<HttpResponseMessage> LogInUserAsync(NamePasswordPair request, bool rememberDevice = false)
        {
            try
            {
                BaseResponse response = null;
                LogInResponse result = await _oblEndpoints.LogInUserAsync(request, rememberDevice);

                if (result.Message == null)
                    response = new ItemResponse<User>(result.User);
                else
                    response = new ItemResponse<string>(result.Message);

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [HttpGet, Route("logout")]
        public HttpResponseMessage LogOutUser()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _oblEndpoints.LogOutUser());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [HttpPost, Route("register")]
        public async Task<HttpResponseMessage> RegisterAsync(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ItemResponse<string> response = await _oblEndpoints.RegisterAsync(user);
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                    throw new Exception("user is invalid...");

            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [HttpGet, Route("user/resendValidationEmail")]
        public HttpResponseMessage ResendValidationEmail(string username)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _oblEndpoints.ResendValidationEmail(username));
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [HttpPut, Route("user"), Intercept("LoggedIn")]
        public HttpResponseMessage UpdateUser(User user)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _oblEndpoints.UpdateUser(user));
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [HttpGet, Route("user/validatePassword"), Intercept("LoggedIn")]
        public HttpResponseMessage ValidatePassword(string password)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _oblEndpoints.ValidatePassword(password));
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [HttpGet, Route("user/tfauth")]
        public HttpResponseMessage ValidateTFAuthCode(string id, string code)
        {
            try
            {
                ItemResponse<User> response = _oblEndpoints.ValidateTFAuthCode(id, code);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        #endregion User Service Endpoints

        #region Bill Service Endpoints

        [HttpDelete, Intercept("LoggedIn"), Route("bill/expenses/{id:int}")]
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
                return ErrorResponse(ex);
            }
        }

        [HttpDelete, Intercept("LoggedIn"), Route("bill/income/{id:int}")]
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
                return ErrorResponse(ex);
            }
        }

        [HttpGet, Route("bill/expenses/all"), Intercept("LoggedIn")]
        public HttpResponseMessage GetAllExpenses()
        {
            try
            {
                List<Expense> result = null;
                result = _billSrv.GetAllExpenses(_userSrv.SessionUser.Id);
                ItemsResponse<Expense> response = new ItemsResponse<Expense>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [HttpGet, Route("bill/income/all"), Intercept("LoggedIn")]
        public HttpResponseMessage GetAllIncome()
        {
            try
            {
                List<Income> result = null;
                result = _billSrv.GetAllIncome(_userSrv.SessionUser.Id);
                ItemsResponse<Income> response = new ItemsResponse<Income>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [HttpGet, Route("bill/combined/chart"), Intercept("LoggedIn")]
        public HttpResponseMessage GetCombinedChart(DateTime? startDate = null
                                                    , DateTime? endDate = null
                                                    , ScheduleTypes chartSchedule = ScheduleTypes.Any)
        {
            try
            {
                Chart<List<float>> result = null;
                result = _billSrv.GetCombinedChart(_userSrv.SessionUser.Id, out chartSchedule, startDate, endDate);
                ItemResponse<Chart<List<float>>> response = new ItemResponse<Chart<List<float>>>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [HttpGet, Route("bill/expenses"), Intercept("LoggedIn")]
        public HttpResponseMessage GetExpense(int id = 0
                                            , string name = null
                                            , ScheduleTypes scheduleType = ScheduleTypes.Any
                                            , ExpenseType billType = ExpenseType.Any)
        {
            try
            {
                Expense result = null;
                result = _billSrv.GetExpense(_userSrv.SessionUser.Id, name);
                ItemResponse<Expense> response = new ItemResponse<Expense>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [HttpGet, Route("bill/expenses/chart"), Intercept("LoggedIn")]
        public HttpResponseMessage GetExpensesChart(DateTime? startDate = null
                                                    , DateTime? endDate = null
                                                    , ScheduleTypes chartSchedule = ScheduleTypes.Any)
        {
            try
            {
                Chart<List<float>> result = null;
                result = _billSrv.GetExpensesChart(_userSrv.SessionUser.Id, out chartSchedule, startDate, endDate);
                ItemResponse<Chart<List<float>>> response = new ItemResponse<Chart<List<float>>>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [HttpGet, Route("bill/income"), Intercept("LoggedIn")]
        public HttpResponseMessage GetIncome(int id = 0
                                            , string name = null
                                            , ScheduleTypes scheduleType = ScheduleTypes.Any
                                            , IncomeType incomeType = IncomeType.Any)
        {
            try
            {
                Income result = null;
                result = _billSrv.GetIncome(_userSrv.SessionUser.Id, name);
                ItemResponse<Income> response = new ItemResponse<Income>(result);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [HttpGet, Route("bill/income/chart"), Intercept("LoggedIn")]
        public HttpResponseMessage GetIncomeChart(DateTime? startDate = null
                                                   , DateTime? endDate = null
                                                   , ScheduleTypes chartSchedule = ScheduleTypes.Any)
        {
            try
            {
                Chart<List<float>> chart = null;
                chart = _billSrv.GetIncomeChart(_userSrv.SessionUser.Id, out chartSchedule, startDate, endDate);
                ItemResponse<Chart<List<float>>> response = new ItemResponse<Chart<List<float>>>(chart);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [HttpPost, Route("bill/expenses"), Intercept("LoggedIn")]
        public HttpResponseMessage InsertExpense(Expense expense)
        {
            try
            {
                BaseResponse response = null;
                expense.UserId = _userSrv.SessionUser.Id;
                expense.ModifiedUserId = _userSrv.SessionUser.Id;

                if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    _billSrv.InsertExpense(expense);
                    response = new SuccessResponse();
                }

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [HttpPost, Route("bill/income"), Intercept("LoggedIn")]
        public HttpResponseMessage InsertIncome(Income income)
        {
            try
            {
                BaseResponse response = null;
                income.UserId = _userSrv.SessionUser.Id;
                income.ModifiedUserId = _userSrv.SessionUser.Id;

                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                else
                {
                    _billSrv.InsertIncome(income);
                    response = new SuccessResponse();
                }

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [Intercept("LoggedIn")]
        [Route("bill/expenses")]
        [HttpPut]
        public HttpResponseMessage UpdateExpense(Expense expense)
        {
            try
            {
                BaseResponse response = null;
                expense.ModifiedUserId = _userSrv.SessionUser.Id;

                if (!ModelState.IsValid)
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
                return ErrorResponse(ex);
            }
        }

        [Intercept("LoggedIn")]
        [Route("bill/income")]
        [HttpPut]
        public HttpResponseMessage UpdateIncome(Income income)
        {
            try
            {
                BaseResponse response = null;
                income.ModifiedUserId = _userSrv.SessionUser.Id;

                if (!ModelState.IsValid)
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
                return ErrorResponse(ex);
            }
        }

        #endregion Bill Service Endpoints

        #region Card Service Endpoints

        [Intercept("LoggedIn")]
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
                return ErrorResponse(ex);
            }
        }

        [Intercept("LoggedIn")]
        [Route("cards/user")]
        [HttpGet]
        public HttpResponseMessage GetAllCardsByUser()
        {
            try
            {
                List<StyledCard> filteredList = _cardSrv.Where(a => a.UserId == _userSrv.SessionUser.Id)?.ToList();
                ItemsResponse<StyledCard> response = new ItemsResponse<StyledCard>(filteredList);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [Intercept("LoggedIn")]
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
                return ErrorResponse(ex);
            }
        }

        [Intercept("LoggedIn")]
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

                    model.UserId = _userSrv.SessionUser.Id;
                    int id = (int)_cardSrv.Insert(model);
                    if (id == 0) { throw new Exception("Insert Failed"); }
                    response = new ItemResponse<int>(id);
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [Intercept("LoggedIn")]
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
                    model.UserId = _userSrv.SessionUser.Id;
                    _cardSrv.Update(model);
                    response = new SuccessResponse();
                }

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        #endregion Card Service Endpoints

        #region Chart Service Endpoints

        [Intercept("LoggedIn")]
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
                return ErrorResponse(ex);
            }
        }

        [Intercept("LoggedIn")]
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
                return ErrorResponse(ex);
            }
        }

        [Intercept("LoggedIn")]
        [Route("charts/user")]
        [HttpGet]
        public HttpResponseMessage GetAllChartsByUser()
        {
            try
            {
                List<Chart<object>> list = _chartsSrv.GetAll();
                List<Chart<object>> filteredList = list?.Where(a => a.UserId == _userSrv.SessionUser.Id).ToList();
                ItemsResponse<Chart<object>> response = new ItemsResponse<Chart<object>>(filteredList);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [Intercept("LoggedIn")]
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
                return ErrorResponse(ex);
            }
        }

        [Intercept("LoggedIn")]
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
                    model.UserId = _userSrv.SessionUser.Id;
                    int id = _chartsSrv.Insert(model, (a) =>
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
                return ErrorResponse(ex);
            }
        }

        [Intercept("LoggedIn")]
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
                    model.UserId = _userSrv.SessionUser.Id;

                    int id = _chartsSrv.Insert(model,
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
                return ErrorResponse(ex);
            }
        }

        [Intercept("LoggedIn")]
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
                    model.UserId = _userSrv.SessionUser.Id;
                    _chartsSrv.Update(model, (a) =>
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
                return ErrorResponse(ex);
            }
        }

        [Intercept("LoggedIn")]
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
                    model.UserId = _userSrv.SessionUser.Id;
                    _chartsSrv.Update(model, (a) =>
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
                return ErrorResponse(ex);
            }
        }

        #endregion Chart Service Endpoints

        #region Product Development Endpoints

        [Route("productDevelopment")]
        [HttpPost]
        public HttpResponseMessage InsertProductDevelopmentRequest(DelevopProductRequest request)
        {
            try
            {
                BaseResponse response = null;

                if (!ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                else
                {
                    ProductDevelopment product = new ProductDevelopment(request);

                    _delevopProductSrv.Insert(product);

                    response = new SuccessResponse();
                }

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        #endregion Product Development Endpoints

        #region Other Endpoints

        [HttpGet, Route("view/code/{fileName}")]
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
                return ErrorResponse(ex);
            }
        }

        [HttpGet, Route("config/enums/{enumString}")]
        public HttpResponseMessage GetEnums(string enumString)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _oblEndpoints.GetEnums(enumString));
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [HttpGet, Route("config/site"), Intercept("LoggedIn")]
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
                return ErrorResponse(ex);
            }
        }

        [HttpPost, Route("send/email")]
        public async Task<HttpResponseMessage> SendEmail(Dictionary<string, string> emailRequest)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _oblEndpoints.SendEmail(emailRequest));
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        #endregion Other Endpoints
    }
}