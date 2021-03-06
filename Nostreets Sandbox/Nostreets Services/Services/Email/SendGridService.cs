﻿using System;
using System.Threading.Tasks;
using Nostreets_Services.Interfaces.Services;
using Nostreets.Extensions.DataControl.Classes;
using Nostreets.Extensions.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Nostreets_Services.Services.Email
{
    public class SendGridService : IEmailService
    {
        public SendGridService(string apiKey)
        {
            _apiKey = apiKey;
        }

        public SendGridService(string apiKey, IDBService<Error> errorLog)
        {
            _errorLog = errorLog;
            _apiKey = apiKey;
        }

        private string _apiKey = null;
        private IDBService<Error> _errorLog = null;

        public async Task<bool> SendAsync(string email, string toAddress, string subject, string messageText, string messageHtml)
        {
            try
            {

                if (messageText == null || messageHtml == "")
                    messageText = subject;

                SendGridClient client = new SendGridClient(_apiKey);
                SendGridMessage msg = new SendGridMessage()
                {
                    From = new EmailAddress(email),
                    Subject = subject,
                    PlainTextContent = messageText,
                    HtmlContent = messageHtml
                };

                msg.AddTo(new EmailAddress(toAddress));
                Response response = await client.SendEmailAsync(msg);
                bool success = response.StatusCode == System.Net.HttpStatusCode.Accepted;
                return success;
            }
            catch (Exception ex)
            {
                if (_errorLog != null)
                    _errorLog.Insert(new Error(ex));

                throw ex;
            }
        }
    }
}