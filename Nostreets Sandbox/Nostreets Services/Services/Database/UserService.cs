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
        public UserService(IEmailService emailSrv, IDBService<User, string> userDBSrv, IDBService<Token> tokenDBSrv)
        {
            _emailService = emailSrv;
            _userDBService = userDBSrv;
            _tokenDBService = tokenDBSrv;
        }

        private IEmailService _emailService = null;
        private IDBService<User, string> _userDBService = null;
        private IDBService<Token> _tokenDBService = null;

        public User SessionUser => SessionManager.HasAnySessions() ? SessionManager.Get<User>(SessionState.User) : null;

        public bool CheckIfUserCanLogIn(string username, string password, out string failureReason)
        {
            failureReason = null;
            bool result = false;
            User user = _userDBService.Where(
                                       a => a.UserName == username ||
                                       a.Contact.PrimaryEmail == username ||
                                       a.Contact.BackupEmail == username
                                       ).FirstOrDefault();


            if (user == null)
                failureReason = "User doesn't exist...";

            else if (!ValidatePassword(user.Password, password))
                failureReason = "Invalid password for " + username + "...";

            else if (!user.Settings.HasVaildatedEmail)
                failureReason = username + "'s email is not validated...";

            else if (user.Settings.TwoFactorAuthEnabled)
            {
                if (user.Settings.TFAuthByPhone)
                {
                    //todo SMSService txt code to users phone
                }
                else
                {
                    //todo SMSService txt code to users email
                }

                failureReason = "2nd Code was sent to " + ((user.Settings.TFAuthByPhone) ? "Phone" : "Email");
            }
            else
                result = true;

            return result;
        }

        public void Delete(string id)
        {
            _userDBService.Delete(id);
        }

        public User Get(string id)
        {
            return _userDBService.Get(id);
        }

        public List<User> GetAll()
        {
            return _userDBService.GetAll();
        }

        public User GetByUsername(string username)
        {
            return _userDBService.Where(a => a.UserName == username).FirstOrDefault();
        }

        public IEnumerable<User> Where(Func<User, bool> predicate)
        {
            return _userDBService.Where(predicate);
        }

        public void LogOut()
        {
            if (SessionManager.HasAnySessions() && SessionManager.Get<bool>(SessionState.IsLoggedOn))
                SessionManager.AbandonSessions();
        }

        public void LogIn(string username, string password, bool rememberDevice = false)
        {
            User user = null;
            if (CheckIfUserCanLogIn(username, password, out string reason))
            {
                user = GetByUsername(username);

                if (rememberDevice && (user.Settings.IPAddresses == null || !user.Settings.IPAddresses.Contains(HttpContext.Current.GetRequestIPAddress())))
                    user.Settings.IPAddresses.AddValues(new Tuple<string, string, string>(HttpContext.Current.GetRequestIPAddress(), username, password));

                SessionManager.Add(new Dictionary<SessionState, object>{
                        { SessionState.IsLoggedOn, true},
                        { SessionState.IsUser, true},
                        { SessionState.LogInTime, DateTime.Now },
                        { SessionState.User, user }
                });
            }
            else
                throw new Exception(reason);

        }

        public async Task<string> RegisterAsync(User user)
        {
            user.Password = Encryption.SimpleEncryptWithPassword(user.Password, WebConfigurationManager.AppSettings["CryptoKey"]);
            string result = _userDBService.Insert(user);
            SessionManager.Add(new Dictionary<SessionState, object>{
                        { SessionState.IsLoggedOn, false},
                        { SessionState.IsUser, true},
                        { SessionState.LogInTime, DateTime.Now },
                        { SessionState.User, user }
                });


            Token token = new Token
            {
                ExpirationDate = DateTime.Now.AddDays(7),
                IsDisabled = false,
                UserId = result,
                ModifiedUserId = result,
                Value = Guid.NewGuid(),
                Name = user.UserName + "'s Registion Email Token",
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                Type = TokenType.EmailValidtion
            };
            string html = HttpContext.Current.Server.MapPath("\\assets\\ValidateEmail.html").ReadFile()
                                     .Replace("{url}", HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)
                                     + "/emailConfirm?token={0}?user={1}".FormatString(token.Value.ToString(), result));

            _tokenDBService.Insert(token);
            if (!await _emailService.SendAsync("no-reply@nostreetssolutions.com"
                              , user.Contact.PrimaryEmail
                              , "Nostreets Sandbox Validation"
                              , "Nostreets Sandbox Validation"
                              , html))
            {
                throw new Exception("Email for registation not sent...");
            };

            return result;
        }

        public bool ValidateEmail(string value)
        {
            bool result = false;
            Token userToken = _tokenDBService.Where(
                a => !a.IsDisabled && a.ExpirationDate > DateTime.Now && a.Type == TokenType.EmailValidtion && a.UserId == SessionUser.Id && a.Value.ToString() == value
            ).FirstOrDefault();

            if (userToken != null)
            {
                result = true;
                userToken.IsDisabled = true;
                userToken.DateModified = DateTime.Now;
                SessionUser.Settings.IsLockedOut = false;
                SessionUser.Settings.HasVaildatedEmail = true;

                if (SessionUser.Settings.IPAddresses == null)
                    SessionUser.Settings.IPAddresses = new List<string> { HttpContext.Current.GetRequestIPAddress() };
                else
                    SessionUser.Settings.IPAddresses.Add(HttpContext.Current.GetRequestIPAddress());

                _userDBService.Update(SessionUser);
                _tokenDBService.Update(userToken);
            }

            return result;
        }

        public bool CheckIfUsernameExist(string username)
        {
            return _userDBService.Where(a => a.UserName == username).FirstOrDefault() != null ? true : false;
        }

        public bool CheckIfEmailExist(string email)
        {
            return _userDBService.Where(a => a.Contact.PrimaryEmail == email || a.Contact.BackupEmail == email).FirstOrDefault() != null ? true : false;
        }

        public bool ValidatePassword(string encyptedPassword, string password)
        {
            return Encryption.SimpleDecryptWithPassword(encyptedPassword, WebConfigurationManager.AppSettings["CryptoKey"]) == password ? true : false;
        }

        public bool ChangeUserEmail(string email, string password)
        {
            throw new NotImplementedException();
        }

        public bool ChangeUserPassword(string newPassword, string oldPassword)
        {
            throw new NotImplementedException();
        }

        public bool UpdateUserSettings(UserSettings settings)
        {
            throw new NotImplementedException();
        }

        public bool UpdateUserContactInfo(Contact settings)
        {
            throw new NotImplementedException();
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
