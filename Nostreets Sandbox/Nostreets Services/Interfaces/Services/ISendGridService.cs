using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Interfaces.Services
{
    public interface ISendGridService
    {
        Task<bool> Send(string Email, string Name, string toAddress, string subject, string messageText, string messageHtml);
    }
}
