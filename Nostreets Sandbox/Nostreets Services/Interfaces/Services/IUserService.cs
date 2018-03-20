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
    public interface IUserService 
    {

        User SessionUser { get; }

        bool CheckIfUserCanLogIn(string username, string password, out string failureReason);
        bool CheckIfUsernameExist(string username);
        bool CheckIfEmailExist(string email);
        User GetByUsername(string username);
        void LogOut();
        void LogIn(string username, string password, bool rememberDevice = false);
        Task<string> RegisterAsync(User user);
        bool ValidateToken(string value, string userId, out string failureReason);
        IEnumerable<User> Where(Func<User, bool> predicate);
        IEnumerable<Token> Where(Func<Token, bool> predicate);
        bool ValidatePassword(string encrptedPassword, string password);
        bool ChangeUserEmail(string email, string password);
        bool ChangeUserPassword(string newPassword, string oldPassword);
        bool UpdateUserSettings(UserSettings settings);
        bool UpdateUserContactInfo(Contact settings);
        Task<bool> ForgotPasswordEmailAsync(string username);
        bool ForgotPasswordValidation(string token, string userId);
        User FirstOrDefault(Func<User, bool> predicate);
        Token FirstOrDefault(Func<Token, bool> predicate);
    }
}
