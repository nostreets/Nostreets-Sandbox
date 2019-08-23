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
using NostreetsExtensions.DataControl.Classes;
using NostreetsExtensions.DataControl.Enums;
using NostreetsExtensions.Extend.Basic;
using NostreetsExtensions.Extend.IOC;
using NostreetsExtensions.Extend.Web;
using NostreetsExtensions.Interfaces;
using NostreetsExtensions.Utilities;
using NostreetsInterceptor;

using NostreetsRouter.Models.Responses;
using OBL_Website.Classes.Domain;
using OBL_Website.Interfaces;
using OBL_Website.Models.Request;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
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
            _oblBoardSrv = _oblBoardSrv.WindsorResolve(container);
        }

        private IOBLBoardService _oblBoardSrv = null;// _oblSrv.WindsorResolve(container);
        private IBillService _billSrv = null;
        private IChartService _chartsSrv = null;
        private IEmailService _emailSrv = null;
        private IUserService _userSrv = null;
        private IDBService<StyledCard, int> _cardSrv = null;
        private IDBService<WebRequestError, int> _errorLog = null;
        private IDBService<ProductDevelopment, int> _delevopProductSrv = null;

        #region Private Members
        private bool IsOnDemendPrintingEmail(string strippedHtml)
        {
            bool result = false;
            Regex printfulOrderMatch = new Regex(@"(Your Order #)\d\d\d\d...............(has been sent out!)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Regex artOfWhereOrderMatch = new Regex(@"(Your order #)(\d|[a-z])(\d|[a-z])(\d|[a-z])(\d|[a-z])(\d|[a-z])( is shipped!)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Regex[] regices = new[] {
                printfulOrderMatch,
                artOfWhereOrderMatch
            };

            foreach (Regex regex in regices)
            {
                result = regex.IsMatch(strippedHtml);
                if (result)
                    break;
            }

            return result;
        }

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

        [Route("checkEmail")]
        [HttpGet]
        public HttpResponseMessage CheckIfEmailExist(string email)
        {
            try
            {
                bool result = _userSrv.CheckIfEmailExist(email);
                return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<bool>(result));
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [Route("checkUsername")]
        [HttpGet]
        public HttpResponseMessage CheckIfUsernameExist(string username)
        {
            try
            {
                bool result = _userSrv.CheckIfUsernameExist(username);
                return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<bool>(result));
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [Route("user/forgotPassword")]
        [HttpGet]
        public HttpResponseMessage ForgotPasswordEmail(string username)
        {
            try
            {
                _userSrv.ForgotPasswordEmailAsync(username);
                return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [Intercept("IsLoggedIn")]
        [Route("user/session")]
        [HttpGet]
        public HttpResponseMessage GetSessionUser()
        {
            try
            {
                if (_userSrv.SessionUser == null)
                    throw new Exception("Session is has not started...");

                return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<User>(_userSrv.SessionUser));
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [Route("login")]
        [HttpPost]
        public async Task<HttpResponseMessage> LogInUserAsync(NamePasswordPair request, bool rememberDevice = false)
        {
            try
            {
                BaseResponse response = null;
                LogInResponse result = await _userSrv.LogInAsync(request, rememberDevice);

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
                _userSrv.LogOut();
                return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [Route("register")]
        [HttpPost]
        public async Task<HttpResponseMessage> RegisterAsync(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string id = await _userSrv.RegisterAsync(user);
                    return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<string>(id));
                }
                else
                    throw new Exception("user is invalid...");

            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [Route("user/resendValidationEmail")]
        [HttpGet]
        public HttpResponseMessage ResendValidationEmail(string username)
        {
            try
            {
                _userSrv.ResendValidationEmailAsync(username);

                return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [Intercept("IsLoggedIn")]
        [Route("user")]
        [HttpPut]
        public HttpResponseMessage UpdateUser(User user)
        {
            try
            {
                if (user.Id != _userSrv.SessionUser.Id)
                    throw new Exception("Targeted user is not the current user...");

                else
                    _userSrv.UpdateUser(user, true);

                return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [Intercept("IsLoggedIn")]
        [Route("user/validatePassword")]
        [HttpGet]
        public HttpResponseMessage ValidatePassword(string password)
        {
            try
            {
                if (!_userSrv.ValidatePassword(_userSrv.SessionUser.Password, password))
                    throw new Exception("Password is invalid...");

                return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [Route("user/tfauth")]
        [HttpGet]
        public HttpResponseMessage ValidateTFAuthCode(string id, string code)
        {
            try
            {
                Token token = _userSrv.ValidateToken(new TokenRequest { TokenId = id, Code = code }, out State state, out string o);

                if (!token.IsValidated)
                    throw new Exception("Code is invalid...");

                return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<User>(_userSrv.SessionUser));
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        #endregion User Service Endpoints

        #region Bill Service Endpoints

        [HttpDelete, Intercept("IsLoggedIn"), Route("bill/expenses/{id:int}")]
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

        [HttpDelete, Intercept("IsLoggedIn"), Route("bill/income/{id:int}")]
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

        [HttpGet, Route("bill/expenses/all"), Intercept("IsLoggedIn")]
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

        [HttpGet, Route("bill/income/all"), Intercept("IsLoggedIn")]
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

        [HttpGet, Route("bill/combined/chart"), Intercept("IsLoggedIn")]
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

        [HttpGet, Route("bill/expenses"), Intercept("IsLoggedIn")]
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

        [HttpGet, Route("bill/expenses/chart"), Intercept("IsLoggedIn")]
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

        [HttpGet, Route("bill/income"), Intercept("IsLoggedIn")]
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

        [HttpGet, Route("bill/income/chart"), Intercept("IsLoggedIn")]
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

        [HttpPost, Route("bill/expenses"), Intercept("IsLoggedIn")]
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

        [HttpPost, Route("bill/income"), Intercept("IsLoggedIn")]
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

        [Intercept("IsLoggedIn")]
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

        [Intercept("IsLoggedIn")]
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

        [Intercept("IsLoggedIn")]
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

        [Intercept("IsLoggedIn")]
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

        [Intercept("IsLoggedIn")]
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

        [Intercept("IsLoggedIn")]
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

        [Intercept("IsLoggedIn")]
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

        [Intercept("IsLoggedIn")]
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

        [Intercept("IsLoggedIn")]
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

        [Intercept("IsLoggedIn")]
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

        [Intercept("IsLoggedIn")]
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

        [Intercept("IsLoggedIn")]
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

        [Intercept("IsLoggedIn")]
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

        [Intercept("IsLoggedIn")]
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

        [Intercept("IsLoggedIn")]
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

        #region OBL Endpoints

        [Intercept("IsLoggedIn"), Intercept("IsBoardMember")]
        [Route("obl/capital/pieChart")]
        [HttpGet]
        public HttpResponseMessage CapitalPieChart(DateTime? cutoffDate = null)
        {
            try
            {
                Chart<decimal> result = _oblBoardSrv.GetPieChart(cutoffDate);
                return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<Chart<decimal>>(result));
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [Intercept("IsLoggedIn"), Intercept("IsBoardMember")]
        [Route("obl/capital")]
        [HttpGet]
        public HttpResponseMessage CapitalOfCompany(DateTime? cutoffDate = null)
        {
            try
            {
                var result = _oblBoardSrv.GetTotalCapitalOfCompany(cutoffDate);
                return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<string, decimal>(result));
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }


        [Intercept("IsLoggedIn"), Intercept("IsBoardMember")]
        [Route("obl/capital/current")]
        [HttpGet]
        public HttpResponseMessage CurrentMembersCapitalOfCompany(DateTime? cutoffDate = null)
        {
            try
            {
                decimal result = _oblBoardSrv.GetMembersCapital(_userSrv.SessionUser, cutoffDate);
                return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<decimal>(result));
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [Intercept("IsLoggedIn"), Intercept("IsBoardMember")]
        [Route("obl/capital")]
        [HttpPost]
        public HttpResponseMessage AddContribution(ContributionRequest request)
        {
            try
            {
                bool success = true;
                BaseResponse response = null;

                switch (request.Type)
                {
                    case ContributionType.Time:
                        TimeContribution timeContibution = request.ToTimeContribution();
                        timeContibution.UserId = _userSrv.SessionUser.Id;
                        timeContibution.ModifiedUserId = _userSrv.SessionUser.Id;
                        _oblBoardSrv.AddContribution(timeContibution);
                        break;

                    case ContributionType.Financial:
                        FinancialContribution financialContibution = request.ToFinancialContribution();
                        financialContibution.UserId = _userSrv.SessionUser.Id;
                        financialContibution.ModifiedUserId = _userSrv.SessionUser.Id;
                        _oblBoardSrv.AddContribution(financialContibution);
                        break;

                    case ContributionType.DigitalAsset:
                        DigitalAssetContribution assetContibution = request.ToDigitalAssetContribution();
                        assetContibution.UserId = _userSrv.SessionUser.Id;
                        assetContibution.ModifiedUserId = _userSrv.SessionUser.Id;
                        _oblBoardSrv.AddContribution(assetContibution);
                        break;

                    case ContributionType.Deduction:
                        CapitalDeduction deduction = request.ToCapitalDeduction();
                        deduction.UserId = _userSrv.SessionUser.Id;
                        deduction.ModifiedUserId = _userSrv.SessionUser.Id;
                        _oblBoardSrv.AddContribution(deduction);
                        break;

                    default:
                        success = false;
                        break;

                }

                if (success)
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                else
                    throw new Exception("Adding the contribution was not successful...");
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [Intercept("IsLoggedIn"), Intercept("IsBoardMember")]
        [Route("obl/capital")]
        [HttpPut]
        public HttpResponseMessage UpdateContribution(ContributionRequest request)
        {
            try
            {
                bool success = true;
                BaseResponse response = null;

                switch (request.Type)
                {
                    case ContributionType.Time:
                        TimeContribution timeContibution = new TimeContribution();
                        request.MapProperties(ref timeContibution);
                        _oblBoardSrv.UpdateContribution(timeContibution);
                        break;

                    case ContributionType.Financial:
                        FinancialContribution financialContibution = new FinancialContribution();
                        request.MapProperties(ref financialContibution);
                        _oblBoardSrv.UpdateContribution(financialContibution);
                        break;

                    case ContributionType.DigitalAsset:
                        DigitalAssetContribution assetContibution = new DigitalAssetContribution();
                        request.MapProperties(ref assetContibution);
                        _oblBoardSrv.UpdateContribution(assetContibution);
                        break;

                    case ContributionType.Deduction:
                        CapitalDeduction deduction = new CapitalDeduction();
                        request.MapProperties(ref deduction);
                        _oblBoardSrv.UpdateContribution(deduction);
                        break;

                    default:
                        success = false;
                        break;

                }

                if (success)
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                else
                    throw new Exception("Adding the contribution was not successful...");
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }


        [Intercept("IsLoggedIn"), Intercept("IsBoardMember")]
        [Route("obl/capital/{id:int}")]
        [HttpDelete]
        public HttpResponseMessage DeleteContribution(int id, ContributionType type)
        {
            try
            {
                _oblBoardSrv.DeleteContribution(id, type);
                return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }
        #endregion OBL Endpoints

        #region Other Endpoints
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
                return ErrorResponse(ex);
            }
        }

        [Route("config/enums/{enumString}")]
        [HttpGet]
        public HttpResponseMessage GetEnums(string enumString)
        {
            try
            {
                string[] enumTypes = enumString.Split(',');
                ItemsResponse<string, Dictionary<int, string>> response = null;
                Dictionary<string, Dictionary<int, string>> result = new Dictionary<string, Dictionary<int, string>>();

                foreach (string _enum in enumTypes)
                {
                    #region Parse N Search For Enum

                    Type enumType = null;
                    string _enumCamel = _enum[0].ToUpperCase().Append(_enum.Where((a, b) => b > 0).ToArray()),
                           _enumCamelWithType = _enum[0].ToUpperCase().Append(_enum.Where((a, b) => b > 0).ToArray()) + "Type",
                           _enumCamelWithTypePlural = _enum[0].ToUpperCase().Append(_enum.Where((a, b) => b > 0).ToArray()) + "Types";

                    enumType = (Type)_enum.ScanAssembliesForObject("Nostreets", ClassTypes.Type, a => a.IsEnum);
                    if (enumType == null)
                        enumType = (Type)_enumCamel.ScanAssembliesForObject("Nostreets", ClassTypes.Type, a => a.IsEnum);
                    if (enumType == null)
                        enumType = (Type)_enumCamelWithType.ScanAssembliesForObject("Nostreets", ClassTypes.Type, a => a.IsEnum);
                    if (enumType == null)
                        enumType = (Type)_enumCamelWithTypePlural.ScanAssembliesForObject("Nostreets", ClassTypes.Type, a => a.IsEnum);

                    #endregion Parse N Search For Enum

                    if (enumType != null)
                    {
                        Dictionary<int, string> types = enumType.EnumToDictionary(),
                                                safeTypes = new Dictionary<int, string>();

                        foreach (var type in types)
                            safeTypes.Add(type.Key, type.Value.SafeName());

                        result.Add(_enum, types);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<string, Dictionary<int, string>>(result));
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        [Intercept("IsLoggedIn")]
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
                return ErrorResponse(ex);
            }
        }

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
                                                + "<div>" + "IP: " + HttpContext.Current.GetIP4Address() + "</div>"
                                                + "<div>" + "Name: " + emailRequest["name"] + "</div>"
                                                + "<div>" + "Email: " + emailRequest["fromEmail"] + "</div>"
                                                + "<div>" + phoneNumber + "</div>"
                                                + "<div>" + "Message: " + emailRequest["messageText"] + "</div>";
                }

                if (!await _emailSrv.SendAsync(emailRequest["fromEmail"], emailRequest["toEmail"], emailRequest["subject"], emailRequest["messageText"], emailRequest["messageHtml"]))
                    throw new Exception("Email Was Not Sent");

                return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        public HttpResponseMessage ExamineEmail(NameValueCollection form)
        {
            var email = new InboundEmail();
            email.Sender = form.Get("sender");

            email.FromEmail = form.Get("sender");
            email.RecipientEmail = form.Get("recipient");
            email.Subject = form.Get("subject");
            email.BodyPlain = form.Get("body-plain");
            email.StrippedText = form.Get("stripped-text");
            email.StrippedSignature = form.Get("stripped-signature");
            email.BodyHtml = form.Get("body-html");
            email.StrippedHtml = form.Get("stripped-html");
            email.AttachmentCount = form.Get("attachment-count");
            email.TimeStamp = form.Get("timestamp");



            if (IsOnDemendPrintingEmail(email.StrippedHtml))
            {
                //todo: Sent Email with tracking number 
            }



            return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
        }

        #endregion Other Endpoints
    }
}