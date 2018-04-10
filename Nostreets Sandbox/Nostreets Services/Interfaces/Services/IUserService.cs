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

        Task<Tuple<User, string>> GetUserWithLoginInfo(string username, string password);
        bool CheckIfUsernameExist(string username);
        bool CheckIfEmailExist(string email);
        User GetByUsername(string username);
        void LogOut();
        Task<Tuple<User, string>> LogInAsync(NamePasswordPair pair, bool rememberDevice = false);
        Task<string> RegisterAsync(User user);
        Token ValidateToken(TokenRequest request, out string output);
        IEnumerable<User> Where(Func<User, bool> predicate);
        IEnumerable<Token> Where(Func<Token, bool> predicate);
        bool ValidatePassword(string encrptedPassword, string password);
        bool ChangeUserPassword(string newPassword, string oldPassword);
        void ForgotPasswordEmailAsync(string username);
        User FirstOrDefault(Func<User, bool> predicate);
        Token FirstOrDefault(Func<Token, bool> predicate);
        void Update(User user, bool encryptPassword = false);
        string Insert(User user);
        string Insert(Token token);
        void EmailNewPasswordAsync(User user, string newPassword);
        void ResendValidationEmailAsync(string username);
    }
}
