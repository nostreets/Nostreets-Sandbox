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
using System.Web.Configuration;
using Nostreets_Services.Interfaces.Services;
using Nostreets_Services.Domain.Charts;
using Nostreets_Services.Models.Request;
using Nostreets_Services.Services.Database;
using Nostreets_Services.Services.Web;
using Nostreets_Sandbox.Models.Responses;
using Nostreets_Sandbox.Controllers.Attributes;

namespace Nostreets_Sandbox.Controllers.Api
{
    [RoutePrefix("api")]
    public class SandoxApiController : ApiController
    {
        IChartsExtended _chartsSrv = null;
        ISendGridService _sendGridSrv = null;

        public SandoxApiController(/*IDBService<Chart, int, ChartAddRequest, ChartUpdateRequest> chartsInject, ISendGridService sendGridInject*/)
        {
            _chartsSrv = UnityConfig.GetContainer().Resolve<ChartsService>();
            _sendGridSrv = UnityConfig.GetContainer().Resolve<SendGridService>();
        }

        private string GetStringWithinLines(int begining, int ending, string[] file) {

            string result = null;
            for (int i = begining + 1; i <= ending; i++) {
                result += "\r\n" + file[i];
            }
            return result;
        }

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
        public HttpResponseMessage InsertChart(ChartAddRequest<int> model)
        {
            try
            {
                if (ModelState.IsValid)
                {
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
        public HttpResponseMessage InsertChart(ChartAddRequest<List<int>> model)
        {
            try
            {
                if (ModelState.IsValid)
                {
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

        [Auth("AzureDBConnection")]
        [Route("charts/int")]
        [HttpPut]
        public HttpResponseMessage UpdateChart(ChartUpdateRequest<int> model)
        {
            try
            {
                if (ModelState.IsValid)
                {
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

        [Auth("AzureDBConnection")]
        [Route("charts/list/int")]
        [HttpPut]
        public HttpResponseMessage UpdateChart(ChartUpdateRequest<List<int>> model)
        {
            try
            {
                if (ModelState.IsValid)
                {
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

        [Auth("AzureDBConnection")]
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


    }
}
