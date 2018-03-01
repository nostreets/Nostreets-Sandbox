using Nostreets_Services.Interfaces.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
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

        public async Task<bool> Send(string email, string toAddress, string subject, string messageText, string messageHtml)
        {
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
    }
}