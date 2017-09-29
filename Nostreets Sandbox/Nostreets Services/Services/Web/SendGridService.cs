using Nostreets_Services.Interfaces.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Nostreets_Services.Services.Web
{
    public class SendGridService : ISendGridService
    {
        public SendGridService(string apiKey)
        {
            ApiKey = apiKey;
        }

        private string ApiKey { get; }

        public async Task<bool> Send(string Email, string Name, string toAddress, string subject, string messageText, string messageHtml)
        {
            SendGridClient client = new SendGridClient(ApiKey);
            SendGridMessage msg = new SendGridMessage()
            {
                From = new EmailAddress(Email, Name),
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