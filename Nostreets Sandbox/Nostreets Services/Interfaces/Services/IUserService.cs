using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Nostreets_Services.Domain.Users;
using NostreetsExtensions.DataControl.Classes;
using NostreetsExtensions.DataControl.Enums;

namespace Nostreets_Services.Interfaces.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Gets the request ip.
        /// </summary>
        /// <value>
        /// The request ip.
        /// </value>
        string RequestIp { get; }
        /// <summary>
        /// Gets the session user.
        /// </summary>
        /// <value>
        /// The session user.
        /// </value>
        User SessionUser { get; }

        /// <summary>
        /// Gets the user with login information.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        Task<LogInResponse> GetUserWithLoginInfo(string username, string password);

        /// <summary>
        /// Checks if username exist.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        bool CheckIfUsernameExist(string username);

        /// <summary>
        /// Checks if email exist.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        bool CheckIfEmailExist(string email);

        List<User> GetAllUsers();
            
        /// <summary>
        /// Gets the by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        User GetByUsername(string username);

        Dictionary<User, UserData> GetAllUsersWithUserData();

        UserData GetUserData(User user);

        /// <summary>
        /// Logs out.
        /// </summary>
        void LogOut();

        /// <summary>
        /// Logs in asynchronous.
        /// </summary>
        /// <param name="pair">The pair.</param>
        /// <param name="rememberDevice">if set to <c>true</c> [remember device].</param>
        /// <returns></returns>
        Task<LogInResponse> LogInAsync(NamePasswordPair pair, bool rememberDevice = false);

        /// <summary>
        /// Registers the asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        Task<string> RegisterAsync(User user);

        /// <summary>
        /// Validates the token.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="state">The state.</param>
        /// <param name="output">The output.</param>
        /// <returns></returns>
        Token ValidateToken(TokenRequest request, out State state, out string output);

        /// <summary>
        /// Wheres the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        IEnumerable<User> Where(Func<User, bool> predicate);

        /// <summary>
        /// Wheres the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        IEnumerable<Token> Where(Func<Token, bool> predicate);

        /// <summary>
        /// Validates the password.
        /// </summary>
        /// <param name="encrptedPassword">The encrpted password.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        bool ValidatePassword(string encrptedPassword, string password);

        /// <summary>
        /// Changes the user password.
        /// </summary>
        /// <param name="newPassword">The new password.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <returns></returns>
        bool ChangeUserPassword(string newPassword, string oldPassword);

        /// <summary>
        /// Forgots the password email asynchronous.
        /// </summary>
        /// <param name="username">The username.</param>
        void ForgotPasswordEmailAsync(string username);

        /// <summary>
        /// Firsts the or default.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        User FirstOrDefault(Func<User, bool> predicate);

        /// <summary>
        /// Firsts the or default.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        Token FirstOrDefault(Func<Token, bool> predicate);

        /// <summary>
        /// Updates the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="encryptPassword">if set to <c>true</c> [encrypt password].</param>
        void UpdateUser(User user, bool encryptPassword = false);

        /// <summary>
        /// Inserts the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        string InsertUser(User user);

        /// <summary>
        /// Inserts the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        string InsertToken(Token token);

        /// <summary>
        /// Emails the new password asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="newPassword">The new password.</param>
        void EmailNewPasswordAsync(User user, string newPassword);

        /// <summary>
        /// Resends the validation email asynchronous.
        /// </summary>
        /// <param name="username">The username.</param>
        void ResendValidationEmailAsync(string username);

        /// <summary>
        /// Inserts the user data.
        /// </summary>
        /// <param name="userData">The user data.</param>
        /// <returns></returns>
        int InsertUserData(UserData userData);

        /// <summary>
        /// Updates the user data.
        /// </summary>
        /// <param name="userData">The user data.</param>
        void UpdateUserData(UserData userData);
         
    }
}