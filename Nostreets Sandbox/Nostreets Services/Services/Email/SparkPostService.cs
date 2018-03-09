using Nostreets_Services.Interfaces.Services;
using SparkPost;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Nostreets_Services.Services.Email
{
    public class SparkPostService : IEmailService
    {
        public SparkPostService(string apiKey)
        {
            _apiKey = apiKey;
        }

        private string _apiKey = null;

        public async Task<bool> Send(string fromEmail, string toEmail, string subject, string messageText, string messageHtml) {

            Options options = new Options { Sandbox = true };
            Transmission transmission = new Transmission();
            transmission.Content.From.Email = "no-reply@sparkpostbox.com";
            transmission.Content.Subject = subject;
            transmission.Content.Text = messageText;
            transmission.Content.Html = messageHtml;
            Recipient recipient = new Recipient
            {
                Address = new Address { Email = toEmail }
            };
            transmission.Recipients.Add(recipient);
            transmission.Options = options;

            Client client = new Client(_apiKey);
            return (await client.Transmissions.Send(transmission) != null) ? true : false;
        }


        #region Legacy
        public async Task<bool> LegacySend(string fromEmail, string toEmail, string subject, string messageText, string messageHtml)
        {
            using (SmtpClient smtpClient = new SmtpClient("smtp.sparkpostmail.com", 587))
            {
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(WebConfigurationManager.AppSettings["SparkPost.Username"], _apiKey);

                using (MailMessage email = new MailMessage())
                {
                    email.IsBodyHtml = true;
                    email.From = new MailAddress(fromEmail);
                    email.To.Add(new MailAddress(toEmail));
                    email.Subject = subject;
                    email.Body = messageHtml;

                    await smtpClient.SendMailAsync(email);
                }
            }

            return true;

        } 
        #endregion
    }
}
