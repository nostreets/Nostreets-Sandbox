using Nostreets_Services.Interfaces.Services;
using NostreetsExtensions;
using SparkPost;
using System;
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

        public async Task<bool> SendAsync(string fromEmail, string toEmail, string subject, string messageText, string messageHtml) {

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
            SendTransmissionResponse response = await client.Transmissions.Send(transmission);

            return response.StatusCode == HttpStatusCode.OK ? true : false;

        }

        public bool Send(string fromEmail, string toEmail, string subject, string messageText, string messageHtml)
        {
            return SendAsync(fromEmail, toEmail, subject, messageText, messageHtml).SyncTask();
        }


        #region Legacy
        public async Task<bool> SendAsyncSMTP(string fromEmail, string toEmail, string subject, string messageText, string messageHtml)
        {
            using (SmtpClient smtpClient = new SmtpClient("smtp.sparkpostmail.com", 587))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Credentials = new NetworkCredential("SMTP_Injection", _apiKey);
                //smtpClient.Credentials = smtpClient.Credentials.GetCredential("smtp.sparkpostmail.com", 587, "AUTH LOGIN");
                smtpClient.EnableSsl = true;

                using (MailMessage email = new MailMessage())
                {
                    email.IsBodyHtml = true;
                    email.From = new MailAddress("no-reply@smtp.sparkpostmail.com");
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
