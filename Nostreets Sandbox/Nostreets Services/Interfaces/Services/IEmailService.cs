using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Interfaces.Services
{
    public interface IEmailService
    {
        Task<bool> Send(string fromEmail, string toEmail, string subject, string messageText, string messageHtml);
    }
}
