using Nostreets_Services.Domain;
using Nostreets_Services.Domain.Base;
using Nostreets_Services.Enums;
using Nostreets_Services.Interfaces.Services;
using Nostreets_Services.Services.Email;
using NostreetsExtensions;
using NostreetsExtensions.Interfaces;
using NostreetsExtensions.Utilities;
using NostreetsORM;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace Nostreets_Services.Services.Database
{
    public class UserService : IUserService
    {
        public UserService(IEmailService emailSrv, IDBService<User, string> userDBSrv, IDBService<Token, string> tokenDBSrv)
        {
            _emailSrv = emailSrv;
            _userDBSrv = userDBSrv;
            _tokenDBSrv = tokenDBSrv;
        }

        private IEmailService _emailSrv = null;
        private IDBService<User, string> _userDBSrv = null;
        private IDBService<Token, string> _tokenDBSrv = null;


        public string RequestIp => HttpContext.Current.GetIPAddress();
        public User SessionUser { get { return GetSessionUser(); } }


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
                    CacheManager.AddOrSet(_sessionUser.Id, _sessionUser, DateTimeOffset.Now.AddHours(2));


                return _sessionUser;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private string DecryptPassword(string encyptedPassword)
        {
            string decryptedPassword = encyptedPassword.Decrypt(WebConfigurationManager.AppSettings["CryptoKey"]);
            decryptedPassword.Log();

            return decryptedPassword;
        }

        private void UpdateCache(User user)
        {
            if (RequestIp != null && !CacheManager.Contains(RequestIp))
                CacheManager.AddOrSet(RequestIp, user.Id);

            CacheManager.AddOrSet(user.Id, user);
        }


        public async Task<Tuple<User, string>> GetUserWithLoginInfo(string username, string password)
        {
            string failureReason = null;
            User user = SessionUser ?? GetByUsername(username);
            bool hasIP = (user == null) ? false : user.Settings.IPAddresses.Contains(RequestIp);



            if (user == null)
                failureReason = "User doesn't exist...";

            else if (!ValidatePassword(user.Password, password))
                failureReason = "Invalid password for " + username + "...";

            else if (!user.Settings.HasVaildatedEmail)
                failureReason = username + "'s email is not validated...";

            else if (user.Settings.ValidateIPBeforeLogin && !hasIP)
                failureReason = username + " does not authorize this computer for login...";

            else if (user.Settings.TwoFactorAuthEnabled)
            {
                Token token = new Token
                {
                    ExpirationDate = DateTime.Now.AddHours(1),
                    ModifiedUserId = user.Id,
                    UserId = user.Id,
                    Type = TokenType.TwoFactorAuth,
                    Value = new Random().RandomString(6),
                    Name = user.UserName + "s' TFAuth Code"
                };

                failureReason = "2auth" + Insert(token);


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


            if (user != null && !hasIP)
            {
                string html = HttpContext.Current.Server.MapPath("\\assets\\emails\\UnindentifiedLogin.html").ReadFile()
                                          .Replace("{user}", user.UserName)
                                          .Replace("{ip}", RequestIp ?? "unknown")
                                          .Replace("{time}", DateTime.Now.ToLongDateString());

                if (!await _emailSrv.SendAsync("no-reply@nostreetssolutions.com"
                              , user.Contact.PrimaryEmail
                              , "Nostreets Sandbox Unindentified Login"
                              , "Nostreets Sandbox Unindentified Login"
                              , html))
                    throw new Exception("Unindentified Login Email Did Not Send...");
            }


            if (failureReason == null)
            {
                user.LastLogIn = DateTime.Now;
            }

            return new Tuple<User, string>(failureReason == null ? user : null, failureReason);
        }

        public void Delete(string id)
        {
            _userDBSrv.Delete(id);
        }

        public List<User> GetAll()
        {
            return _userDBSrv.GetAll();
        }

        public User GetByUsername(string username)
        {
            return FirstOrDefault(a => a.UserName == username || a.Contact.PrimaryEmail == username);
        }

        public async Task<Tuple<User, string>> LogInAsync(NamePasswordPair pair, bool rememberDevice = false)
        {
            string tokenId = null;
            Tuple<User, string> validUser = await GetUserWithLoginInfo(pair.Username, pair.Password);

            if (validUser.Item2 != null)
                if (validUser.Item2.Contains("2auth"))
                    tokenId = validUser.Item2.Substring(5);
                else
                    throw new Exception(validUser.Item2);

            else if (validUser.Item1 != null)
            {
                if (rememberDevice && !validUser.Item1.Settings.IPAddresses.Contains(RequestIp))
                    validUser.Item1.Settings.IPAddresses.Add(RequestIp);

                Update(validUser.Item1);
            }

            return new Tuple<User, string>(validUser.Item1, tokenId);
        }

        public async Task<string> RegisterAsync(User user)
        {
            user.Id = Insert(user);

            Token token = new Token
            {
                ExpirationDate = DateTime.Now.AddDays(7),
                IsDeleted = false,
                UserId = user.Id,
                ModifiedUserId = user.Id,
                Name = user.UserName + "'s Registion Email Token",
                Type = TokenType.EmailValidtion
            };

            token.Id = Insert(token);

            string html = HttpContext.Current.Server.MapPath("\\assets\\emails\\ValidateEmail.html").ReadFile()
                                     .Replace("{url}", HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)
                                     + "?token={0}&user={1}".FormatString(token.Id, user.Id));


            if (!await _emailSrv.SendAsync("no-reply@nostreetssolutions.com"
                              , user.Contact.PrimaryEmail
                              , "Nostreets Sandbox Email Validation"
                              , "Nostreets Sandbox Email Validation"
                              , html))
                throw new Exception("Email for registation not sent...");

            return user.Id;
        }

        public Token ValidateToken(TokenRequest request, out string output)
        {
            output = "";
            Token token = _tokenDBSrv.Get(request.TokenId);
            bool updateUser = false;


            if (token == null)
                output = "Token does not exist...";

            else if (token.ExpirationDate < DateTime.Now)
                output = "Token is expired...";

            else if (token.IsDeleted)
                output = "Token no longer exists...";

            else if (request.UserId != null && token.UserId != request.UserId)
                output = "Token does not belong to specified user...";

            else
            {
                User user = SessionUser ?? _userDBSrv.Get(request.UserId ?? token.UserId);
                token.DateModified = DateTime.Now;


                switch (token.Type)
                {
                    case TokenType.EmailValidtion:
                        user.Settings.HasVaildatedEmail = true;
                        user.Settings.IsLockedOut = false;
                        token.IsValidated = true;
                        token.IsDeleted = true;
                        output = "Email Validated...";
                        updateUser = true;
                        break;

                    case TokenType.PasswordReset:
                        EmailNewPasswordAsync(user, token.Value);
                        token.IsValidated = true;
                        token.IsDeleted = true;
                        output = "New Password Was Emailed...";
                        break;

                    case TokenType.PhoneValidtion:
                        user.Settings.HasVaildatedPhone = true;
                        token.IsValidated = true;
                        token.IsDeleted = true;
                        output = "Phone Validated...";
                        updateUser = true;
                        break;

                    case TokenType.TwoFactorAuth:
                        token.IsValidated = (token.Value == request.Code) ? true : false;
                        token.IsDeleted = (token.Value == request.Code) ? true : false;
                        output = "Code Validated...";
                        updateUser = true;
                        break;

                }

                _tokenDBSrv.Update(token);

                if (updateUser)
                    Update(user);

            }

            return token;
        }

        public bool CheckIfUsernameExist(string username)
        {
            return _userDBSrv.Where(a => a.UserName == username).FirstOrDefault() != null ? true : false;
        }

        public bool CheckIfEmailExist(string email)
        {
            return _userDBSrv.Where(a => a.Contact.PrimaryEmail == email).FirstOrDefault() != null ? true : false;
        }

        public bool ValidatePassword(string encyptedPassword, string password)
        {
            return DecryptPassword(encyptedPassword) == password ? true : false;
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

        public void LogOut()
        {
            _userDBSrv.Update(SessionUser);
            HttpContext.Current.SetCookie("loggedIn", "false");
            CacheManager.Remove(CacheManager.Get<string>(RequestIp));
            CacheManager.Remove(RequestIp);
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
                    Type = TokenType.PasswordReset
                };
                user.Password = token.Value;


                token.Id = Insert(token);


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

        public IEnumerable<User> Where(Func<User, bool> predicate)
        {
            return _userDBSrv.Where(predicate);
        }

        public IEnumerable<Token> Where(Func<Token, bool> predicate)
        {
            return _tokenDBSrv.Where(predicate);
        }

        public User FirstOrDefault(Func<User, bool> predicate)
        {
            return _userDBSrv.FirstOrDefault(predicate);
        }

        public Token FirstOrDefault(Func<Token, bool> predicate)
        {
            return _tokenDBSrv.FirstOrDefault(predicate);
        }

        public void Update(User user, bool encryptPassword = false)
        {

            if (encryptPassword)
                user.Password = user.Password.Encrypt(WebConfigurationManager.AppSettings["CryptoKey"]);

            UpdateCache(user);

            _userDBSrv.Update(user);
        }

        public string Insert(User user)
        {
            user.Settings = new UserSettings
            {
                IPAddresses = new List<string> { RequestIp }
            };
            user.Password = user.Password.Encrypt(WebConfigurationManager.AppSettings["CryptoKey"]);
            user.Id = _userDBSrv.Insert(user);


            if (!CacheManager.Contains(RequestIp))
                CacheManager.AddOrSet(RequestIp, user.Id);
            CacheManager.AddOrSet(user.Id, user);

            return user.Id;
        }

        public string Insert(Token token)
        {
            token.UserId = token.UserId ?? SessionUser?.Id;
            token.ModifiedUserId = token.UserId ?? SessionUser?.Id;
            token.Id = _tokenDBSrv.Insert(token);

            return token.Id;
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
                Update(user);
            }
            else
                throw new Exception("New Password Email Did Not Send...");
        }

        public async void ResendValidationEmailAsync(string username)
        {
            User user = GetByUsername(username);

            _tokenDBSrv.Delete(
                _tokenDBSrv.Where(
                    a => a.Type == TokenType.EmailValidtion && a.UserId == user.Id).Select(a => a.Id));


            Token token = new Token
            {
                ExpirationDate = DateTime.Now.AddDays(7),
                IsDeleted = false,
                UserId = user.Id,
                ModifiedUserId = user.Id,
                Name = user.UserName + "'s Registion Email Token",
                Type = TokenType.EmailValidtion
            };
            token.Id = Insert(token);

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
    }

    #region Legacy
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

        UserDBContext _context = null;

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

        public User GetByUsername(string username)
        {
            User user = _context.Users.FirstOrDefault(a => a.UserName == username);
            return user;
        }

        public List<User> GetAll()
        {
            return _context.Users.ToList();
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
    #endregion
}
