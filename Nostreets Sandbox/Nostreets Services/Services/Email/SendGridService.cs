using Nostreets_Services.Interfaces.Services;
using NostreetsExtensions;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace Nostreets_Services.Services.Email
{
    public class SendGridService : IEmailService
    {
        public SendGridService(string apiKey)
        {
            ApiKey = apiKey;
        }

        private string ApiKey { get; }

        public bool Send(string fromEmail, string toEmail, string subject, string messageText, string messageHtml)
        {
            return SendAsync(fromEmail, toEmail, subject, messageText, messageHtml).SyncTask();
        }

        public async Task<bool> SendAsync(string email, string toAddress, string subject, string messageText, string messageHtml)
        {
            try
            {

                if (messageText == null || messageHtml == "")
                    messageText = subject;

                SendGridClient client = new SendGridClient(ApiKey);
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
                throw ex;
            }
        }
    }
}