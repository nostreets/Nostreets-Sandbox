using Nostreets_Services.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Nostreets_Services.Services.Email
{
    public class SparkPostService : IEmailService
    {
        public SparkPostService(string apiKey)
        {
            apiKey = _apiKey;
        }

        private string _apiKey = null;

        public async Task<bool> Send(string fromEmail, string toEmail, string subject, string messageText, string messageHtml)
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
    }
}
