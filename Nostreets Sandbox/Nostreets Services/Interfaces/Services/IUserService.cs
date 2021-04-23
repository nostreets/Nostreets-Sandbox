using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Nostreets.Extensions.DataControl.Classes;
using Nostreets.Extensions.DataControl.Enums;

using Nostreets_Services.Classes.Domain.Users;

namespace Nostreets_Services.Interfaces.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserService
    {

        string RequestIp { get; }
        User SessionUser { get; }
        bool ChangeUserPassword(string newPassword, string oldPassword);

        bool CheckIfEmailExist(string email);

        bool CheckIfUsernameExist(string username);

        void DeleteUser(string id);

        void EmailNewPasswordAsync(User user, string newPassword);

        User FirstOrDefault(Func<User, bool> predicate);

        Token FirstOrDefault(Func<Token, bool> predicate);

        void ForgotPasswordEmailAsync(string username);

        List<User> GetAllUsers();

        Dictionary<User, UserData> GetAllUsersWithUserData();

        User GetUserById(string userId);

        User GetUserByUsername(string username);

        UserData GetUserDataByUserId(string userId);

        IEnumerable<UserData> GetUsersDataByIP(string IPAddress);

        Task<LogInResponse> GetUserWithLoginInfo(string username, string password);

        string InsertToken(Token token);

        string InsertUser(User user);

        int InsertUserData(UserData userData);

        Task<LogInResponse> LogInAsync(NamePasswordPair pair, bool rememberDevice = false);

        void LogOut();

        Task<string> RegisterAsync(User user);

        void ResendValidationEmailAsync(string username);

        void UpdateUser(User user, bool encryptPassword = false);

        void UpdateUserData(UserData userData);

        bool ValidatePassword(string encyptedPassword, string password);

        Token ValidateToken(TokenRequest request, out State state, out string output);

        IEnumerable<User> GetUsersWhere(Func<User, bool> predicate);

        IEnumerable<Token> GetTokensWhere(Func<Token, bool> predicate);

    }
}