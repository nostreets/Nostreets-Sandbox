using Nostreets_Services.Domain;
using Nostreets_Services.Domain.Base;
using NostreetsExtensions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Interfaces.Services
{
    public interface IUserService : IDBService<User, string>
    {
        bool CheckIfUserCanLogIn(string username, string password, out string failureReason);
        User GetByUsername(string username);
        void LogOut();
        void LogIn(string username, string password, bool rememberDevice = false);
        string Register(User user);
        bool ValidateEmail(Token token);

    }
}
