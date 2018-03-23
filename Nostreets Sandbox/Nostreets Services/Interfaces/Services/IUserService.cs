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
        string RequestIp { get; }
        User SessionUser { get; }

        bool CheckIfUserCanLogIn(string username, string password, out string failureReason);
        bool CheckIfUsernameExist(string username);
        bool CheckIfEmailExist(string email);
        User GetByUsername(string username);
        void LogOut();
        void LogIn(string username, string password, bool rememberDevice = false);
        Task<string> RegisterAsync(User user);
        Token ValidateToken(string tokenId, string userId, out string output);
        IEnumerable<User> Where(Func<User, bool> predicate);
        IEnumerable<Token> Where(Func<Token, bool> predicate);
        bool ValidatePassword(string encrptedPassword, string password);
        bool ChangeUserEmail(string email, string password);
        bool ChangeUserPassword(string newPassword, string oldPassword);
        Task<bool> ForgotPasswordEmailAsync(string username);
        bool ForgotPasswordValidation(string token, string userId);
        User FirstOrDefault(Func<User, bool> predicate);
        Token FirstOrDefault(Func<Token, bool> predicate);
        void Update(User user);
        void Update(Token token);
        string Insert(User user);
        string Insert(Token token);
    }
}
