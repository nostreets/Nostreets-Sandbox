using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

using Nostreets_Services.Domain.Users;
using Nostreets_Services.Enums;
using Nostreets_Services.Interfaces.Services;

using NostreetsExtensions.DataControl.Classes;
using NostreetsExtensions.DataControl.Enums;
using NostreetsExtensions.Extend.Basic;
using NostreetsExtensions.Extend.Web;
using NostreetsExtensions.Interfaces;
using NostreetsExtensions.Utilities;

namespace Nostreets_Services.Services.Database
{
    public class UserService : IUserService
    {
        public UserService(
              HttpContext context
            , IEmailService emailSrv
            , IDBService<User, string> userDBSrv
            , IDBService<UserData, int> idDBSrv
            , IDBService<Token, string> tokenDBSrv)
        {
            _context = context;
            _emailSrv = emailSrv;
            _userDBSrv = userDBSrv;
            _userDataDBSrv = idDBSrv;
            _tokenDBSrv = tokenDBSrv;
        }

        private HttpContext _context = null;
        private IEmailService _emailSrv = null;
        private IDBService<Token, string> _tokenDBSrv = null;
        private IDBService<UserData, int> _userDataDBSrv = null;
        private IDBService<User, string> _userDBSrv = null;
        public string RequestIp => _context.GetIPAddress();
        public User SessionUser { get { return GetSessionUser(); } }

        private string DecryptPassword(string encyptedPassword)
        {
            string decryptedPassword = encyptedPassword.Decrypt(WebConfigurationManager.AppSettings["CryptoKey"]);
            decryptedPassword.LogInDebug();

            return decryptedPassword;
        }

        private User GetSessionUser(string ip = null)
        {
            try
            {
                ip = ip ?? RequestIp;
                User _sessionUser = null;
                string userId = CacheManager.Get<string>(ip);

                if (userId != null)
                    if (CacheManager.Contains(userId))
                        _sessionUser = CacheManager.Get<User>(userId);
                    else
                        _sessionUser = _userDBSrv.Get(CacheManager.Get<string>(ip));

                if (_sessionUser != null && !CacheManager.Contains(_sessionUser.Id))
                    CacheManager.Set(_sessionUser.Id, _sessionUser, 120);

                return _sessionUser;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateCache(User user)
        {
            if (RequestIp != null && !CacheManager.Contains(RequestIp))
                CacheManager.Set(RequestIp, user.Id);

            CacheManager.Set(user.Id, user);
        }

        public bool ChangeUserPassword(string newPassword, string oldPassword)
        {
            bool result = false;
            User user = SessionUser;

            if (user != null && ValidatePassword(user.Password, oldPassword))
            {
                user.Password = newPassword.Encrypt(WebConfigurationManager.AppSettings["CryptoKey"]);
                _userDBSrv.Update(user);
            }

            return result;
        }

        public bool CheckIfEmailExist(string email)
        {
            return _userDBSrv.Where(a => a.Contact.PrimaryEmail == email).FirstOrDefault() != null ? true : false;
        }

        public bool CheckIfUsernameExist(string username)
        {
            return _userDBSrv.Where(a => a.UserName == username).FirstOrDefault() != null ? true : false;
        }

        public void DeleteUser(string id)
        {
            _userDBSrv.Delete(id);
        }

        public async void EmailNewPasswordAsync(User user, string newPassword)
        {
            string html = HttpContext.Current.Server.MapPath("\\assets\\emails\\NewPasswordEmail.html").ReadFile()
                                                    .Replace("{user}", user.UserName)
                                                    .Replace("{password}", newPassword);

            if (await _emailSrv.SendAsync("no-reply@nostreetssolutions.com"
                              , user.Contact.PrimaryEmail
                              , "New Password to Nostreets Sandbox"
                              , "New Password to Nostreets Sandbox"
                              , html))
            {
                user.Password = newPassword.Encrypt(WebConfigurationManager.AppSettings["CryptoKey"]);
                UpdateUser(user);
            }
            else
                throw new Exception("New Password Email Did Not Send...");
        }

        public User FirstOrDefault(Func<User, bool> predicate)
        {
            return _userDBSrv.FirstOrDefault(predicate);
        }

        public Token FirstOrDefault(Func<Token, bool> predicate)
        {
            return _tokenDBSrv.FirstOrDefault(predicate);
        }

        public async void ForgotPasswordEmailAsync(string username)
        {
            if (_userDBSrv.Where(a => a.UserName == username || a.Contact.PrimaryEmail == username) != null)
            {
                User user = _userDBSrv.Where(a => a.UserName == username || a.Contact.PrimaryEmail == username).FirstOrDefault();
                Token token = new Token
                {
                    ExpirationDate = DateTime.Now.AddHours(1),
                    UserId = user.Id,
                    ModifiedUserId = user.Id,
                    Value = new Random().RandomString(12),
                    Name = user.UserName + "'s Password Reset Token",
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    Purpose = TokenPurpose.PasswordReset
                };
                user.Password = token.Value;

                token.Id = InsertToken(token);

                string html = HttpContext.Current.Server.MapPath("\\assets\\emails\\ForgotPasswordEmail.html").ReadFile()
                                                 .Replace("{url}", HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)
                                                 + "?token={0}&user={1}".FormatString(token.Id, user.Id));

                if (!await _emailSrv.SendAsync("no-reply@nostreetssolutions.com"
                              , user.Contact.PrimaryEmail
                              , "Forgot Password to Nostreets Sandbox"
                              , "Forgot Password to Nostreets Sandbox"
                              , html))
                    throw new Exception("Forgot Password Email did not send...");
            }
        }

        public List<User> GetAllUsers()
        {
            return _userDBSrv.GetAll();
        }

        public User GetByUsername(string username)
        {
            return FirstOrDefault(a => a.UserName == username || a.Contact.PrimaryEmail == username);
        }

        public void GetUserData(User user)
        {
            _userDataDBSrv.FirstOrDefault(a => a.UserId == user.Id);
        }

        public void GetUserData(UserData userData)
        {
            _userDataDBSrv.FirstOrDefault(a => a.UserId == userData.UserId);
        }

        public async Task<LogInResponse> GetUserWithLoginInfo(string username, string password)
        {
            string message = null;
            State state = State.Success;
            User user = SessionUser ?? GetByUsername(username);
            UserData associatedData = _userDataDBSrv.FirstOrDefault(a => a.UserId == user?.Id);

            bool hasIP = (user == null) ? false : associatedData.IPAddresses.Contains(RequestIp);

            if (user == null)
            {
                message = "User doesn't exist...";
                state = State.Error;
            }
            else if (!ValidatePassword(user.Password, password))
            {
                message = "Invalid password for " + username + "...";
                state = State.Error;
            }
            else if (!user.Settings.HasVaildatedEmail)
            {
                message = username + "'s email is not validated...";
                state = State.Error;
            }
            else if (user.Settings.ValidateIPBeforeLogin && !hasIP)
            {
                message = username + " does not authorize this computer for login...";
                state = State.Error;
            }
            else if (user.Settings.TwoFactorAuthEnabled)
            {
                Token token = new Token
                {
                    ExpirationDate = DateTime.Now.AddHours(1),
                    ModifiedUserId = user.Id,
                    UserId = user.Id,
                    Purpose = TokenPurpose.TwoFactorAuth,
                    Value = new Random().RandomString(6),
                    Name = user.UserName + "s' TFAuth Code"
                };

                message = "2auth" + InsertToken(token);
                state = State.Info;

                if (user.Settings.TFAuthByPhone)
                {
                    //todo SMSService txt code to users phone
                }
                else
                {
                    string html = HttpContext.Current.Server.MapPath("\\assets\\emails\\TFAuthEmail.html").ReadFile()
                                                            .Replace("{code}", token.Value);

                    if (!await _emailSrv.SendAsync("no-reply@nostreetssolutions.com"
                              , user.Contact.PrimaryEmail
                              , "Login Code to Nostreets Sandbox"
                              , "Login Code to Nostreets Sandbox"
                              , html))
                        throw new Exception("Login Code Email Did Not Send...");
                }
            }

            if (message == null)
                user.LastLogIn = DateTime.Now;

            return new LogInResponse()
            {
                User = message == null ? user : null,
                Message = message,
                State = state
            };
        }

        public string InsertToken(Token token)
        {
            token.UserId = token.UserId ?? SessionUser?.Id;
            token.ModifiedUserId = token.UserId ?? SessionUser?.Id;
            token.Id = _tokenDBSrv.Insert(token);

            return token.Id;
        }

        public string InsertUser(User user)
        {
            UserData userData = null;

            if (user.Settings == null)
                user.Settings = new UserSettings();


            if (user.UserOrigin != UserOriginType.Manual)
                userData = new UserData()
                {
                    ApiIDs = new Dictionary<string, string>()
                    {
                        { user.UserOrigin.ToString() + "Id", user.Password }
                    },
                    IPAddresses = new List<string>() {
                        RequestIp
                    }
                };
            else
                userData = new UserData()
                {
                    IPAddresses = new List<string>() {
                        RequestIp
                    }
                };


            user.Password = user.Password.Encrypt(WebConfigurationManager.AppSettings["CryptoKey"]);
            user.Id = _userDBSrv.Insert(user);

            userData.UserId = user.Id;
            userData.ModifiedUserId = user.Id;

            InsertUserData(userData);

            if (!CacheManager.Contains(RequestIp))
                CacheManager.Set(RequestIp, user.Id);
            CacheManager.Set(user.Id, user);

            return user.Id;
        }

        public int InsertUserData(UserData userData)
        {
            return _userDataDBSrv.Insert(userData);
        }

        public async Task<LogInResponse> LogInAsync(NamePasswordPair pair, bool rememberDevice = false)
        {
            LogInResponse result = await GetUserWithLoginInfo(pair.Username, pair.Password);
            UserData userData = _userDataDBSrv.FirstOrDefault(a => a.UserId == result.User?.Id);

            if (result.State == State.Error)
                throw new Exception(result.Message);

            else if (result.Message.Contains("2auth"))
                result.Message = result.Message.Substring(5);

            else if (result.User != null)
            {
                if (!userData.IPAddresses.Contains(RequestIp))
                {
                    string html = HttpContext.Current.Server.MapPath("\\assets\\emails\\UnindentifiedLogin.html").ReadFile()
                                              .Replace("{user}", result.User.UserName)
                                              .Replace("{ip}", RequestIp ?? "unknown")
                                              .Replace("{time}", DateTime.Now.ToLongDateString());

                    if (rememberDevice)
                        userData.IPAddresses.Add(RequestIp);

                    if (!await _emailSrv.SendAsync("no-reply@nostreetssolutions.com"
                                  , result.User.Contact.PrimaryEmail
                                  , "Nostreets Sandbox Unindentified Login"
                                  , "Nostreets Sandbox Unindentified Login"
                                  , html))
                        throw new Exception("Unindentified Login Email Did Not Send...");
                }

                UpdateUser(result.User);
                UpdateUserData(userData);
            }

            return result;
        }

        public void LogOut()
        {
            _userDBSrv.Update(SessionUser);
            HttpContext.Current.SetCookie("loggedIn", "false");
            CacheManager.Remove(CacheManager.Get<string>(RequestIp));
            CacheManager.Remove(RequestIp);
        }

        public async Task<string> RegisterAsync(User user)
        {
            user.Id = InsertUser(user);

            Token token = new Token
            {
                ExpirationDate = DateTime.Now.AddDays(7),
                IsDeleted = false,
                UserId = user.Id,
                ModifiedUserId = user.Id,
                Name = user.UserName + "'s Registion Email Token",
                Purpose = TokenPurpose.EmailValidtion
            };

            token.Id = InsertToken(token);

            string html = HttpContext.Current.Server.MapPath("\\assets\\emails\\ValidateEmail.html").ReadFile()
                                     .Replace("{url}", HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)
                                     + "?token={0}&user={1}".FormatString(token.Id, user.Id));

            await _emailSrv.SendAsync(
                            "no-reply@nostreetssolutions.com"
                          , user.Contact.PrimaryEmail
                          , "Nostreets Sandbox Email Validation"
                          , "Nostreets Sandbox Email Validation"
                          , html);

            return user.Id;
        }

        public async void ResendValidationEmailAsync(string username)
        {
            User user = GetByUsername(username);

            _tokenDBSrv.Delete(
                _tokenDBSrv.Where(
                    a => a.Purpose == TokenPurpose.EmailValidtion && a.UserId == user.Id).Select(a => a.Id));

            Token token = new Token
            {
                ExpirationDate = DateTime.Now.AddDays(7),
                IsDeleted = false,
                UserId = user.Id,
                ModifiedUserId = user.Id,
                Name = user.UserName + "'s Registion Email Token",
                Purpose = TokenPurpose.EmailValidtion
            };
            token.Id = InsertToken(token);

            string html = HttpContext.Current.Server.MapPath("\\assets\\emails\\ValidateEmail.html").ReadFile()
                                     .Replace("{url}", HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)
                                     + "?token={0}&user={1}".FormatString(token.Id, user.Id));

            if (!await _emailSrv.SendAsync("no-reply@nostreetssolutions.com"
                              , user.Contact.PrimaryEmail
                              , "Nostreets Sandbox Email Validation"
                              , "Nostreets Sandbox Email Validation"
                              , html))
                throw new Exception("Email for registation not sent...");
        }

        public void UpdateUser(User user, bool encryptPassword = false)
        {
            if (encryptPassword)
                user.Password = user.Password.Encrypt(WebConfigurationManager.AppSettings["CryptoKey"]);

            UpdateCache(user);

            _userDBSrv.Update(user);
        }

        public void UpdateUserData(UserData userData)
        {
            _userDataDBSrv.Update(userData);
        }

        public bool ValidatePassword(string encyptedPassword, string password)
        {
            return DecryptPassword(encyptedPassword) == password ? true : false;
        }

        public Token ValidateToken(TokenRequest request, out State state, out string output)
        {
            output = "";
            state = State.Error;

            Token token = _tokenDBSrv.Get(request.TokenId);
            bool updateUser = false;

            if (token == null)
            {
                output = "Token does not exist...";
                state = State.Question;
            }
            else if (token.ExpirationDate < DateTime.Now)
            {
                output = "Token is expired...";
                state = State.Error;
            }
            else if (token.IsDeleted)
            {
                output = "Token no longer exists...";
                state = State.Error;
            }
            else if (request.UserId != null && token.UserId != request.UserId)
            {
                output = "Token does not belong to specified user...";
                state = State.Error;
            }
            else
            {
                state = State.Success;
                User user = SessionUser ?? _userDBSrv.Get(request.UserId ?? token.UserId);
                token.DateModified = DateTime.Now;

                switch (token.Purpose)
                {
                    case TokenPurpose.EmailValidtion:
                        user.Settings.HasVaildatedEmail = true;
                        user.Settings.IsLockedOut = false;
                        token.IsValidated = true;
                        token.IsDeleted = true;
                        output = "Email Validated...";
                        updateUser = true;
                        break;

                    case TokenPurpose.PasswordReset:
                        EmailNewPasswordAsync(user, token.Value);
                        token.IsValidated = true;
                        token.IsDeleted = true;
                        output = "New Password Was Emailed...";
                        break;

                    case TokenPurpose.PhoneValidtion:
                        user.Settings.HasVaildatedPhone = true;
                        token.IsValidated = true;
                        token.IsDeleted = true;
                        output = "Phone Validated...";
                        updateUser = true;
                        break;

                    case TokenPurpose.TwoFactorAuth:
                        token.IsValidated = (token.Value == request.Code) ? true : false;
                        token.IsDeleted = (token.Value == request.Code) ? true : false;
                        output = "Code Validated...";
                        updateUser = true;
                        break;
                }

                _tokenDBSrv.Update(token);

                if (updateUser)
                    UpdateUser(user);
            }

            return token;
        }

        public IEnumerable<User> Where(Func<User, bool> predicate)
        {
            return _userDBSrv.Where(predicate);
        }

        public IEnumerable<Token> Where(Func<Token, bool> predicate)
        {
            return _tokenDBSrv.Where(predicate);
        }
    }

    #region Legacy

    public class UserDBContext : DbContext
    {
        public UserDBContext()
            : base("DefaultConnection" /*"AzureDBConnection"*/)
        {
        }

        public UserDBContext(string connectionKey)
            : base(connectionKey)
        {
            OnModelCreating(new DbModelBuilder());
        }

        public IDbSet<User> Users { get; set; }
    }

    public class UserEFService
    {
        public UserEFService()
        {
            _context = new UserDBContext("DefaultConnection");
        }

        public UserEFService(string connectionKey)
        {
            //var config = new UserMigrationConfig(); //DbMigrationsConfiguration<UserDBContext>();// { AutomaticMigrationsEnabled = true };
            //var migrator = new DbMigrator(config);
            //migrator.Update();

            _context = new UserDBContext(connectionKey);
        }

        private UserDBContext _context = null;

        public bool CheckIfUserExist(string username)
        {
            if (GetByUsername(username) == default(User)) { return false; }
            else { return true; }
        }

        public void Delete(string id)
        {
            User user = _context.Users.FirstOrDefault(a => a.Id == id);

            _context.Users.Remove(user);

            if (_context.SaveChanges() == 0) { throw new Exception("DB changes not saved!"); }
        }

        public User Get(string id)
        {
            User user = _context.Users.FirstOrDefault(a => a.Id == id);
            return user;
        }

        public List<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public User GetByUsername(string username)
        {
            User user = _context.Users.FirstOrDefault(a => a.UserName == username);
            return user;
        }

        public string Insert(User model)
        {
            model.Id = Guid.NewGuid().ToString();
            _context.Users.Add(model);

            if (_context.SaveChanges() == 0) { throw new Exception("DB changes not saved!"); }

            return model.Id;
        }

        public void Update(User model)
        {
            User targetedUser = _context.Users.FirstOrDefault(a => a.Id == model.Id);
            targetedUser = model;

            if (_context.SaveChanges() == 0) { throw new Exception("DB changes not saved!"); }
        }

        public IEnumerable<User> Where(Func<User, bool> predicate)
        {
            return GetAll().Where(predicate);
        }
    }

    #endregion Legacy
}