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
using System.Web;
using System.Web.Configuration;

namespace Nostreets_Services.Services.Database
{
    public class UserService : IUserService
    {
        public UserService(IEmailService emailSrv, IDBService<User, string> userDBSrv, IDBService<Token> tokenDBSrv)
        {
            _emailService = emailSrv;// new SendGridService(WebConfigurationManager.AppSettings["SendGrid.ApiKey"]);
            _userDBService = userDBSrv;// new DBService<User, string>();
            _tokenDBService = tokenDBSrv;// new DBService<Token>();
        }

        private IEmailService _emailService = null;
        private IDBService<User, string> _userDBService = null;
        private IDBService<Token> _tokenDBService = null;


        public bool CheckIfUserCanLogIn(string username, string password, out string failureReason)
        {
            failureReason = null;
            bool result = false;
            string encryptedPassword = Encryption.SimpleEncryptWithPassword(password, password);
            User user = _userDBService.Where(a => a.UserName == username).FirstOrDefault();
            if (user == null)
                failureReason = "User doesn't exist...";

            else if (user.Password != encryptedPassword)
                failureReason = "Invalid password for " + username + "...";

            else if (!user.Settings.HasVaildatedEmail)
                failureReason = username + "'s email is not validated...";

            else if (user.Settings.TwoFactorAuthEnabled)
            {
                if (user.Settings.TFAuthByPhone)
                {
                    //todo SMSService txt code dto users phone
                }
                else
                {
                    //todo SMSService txt code dto users email
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

        public string Insert(User model)
        {
            return _userDBService.Insert(model);
        }

        public void Update(User model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> Where(Func<User, bool> predicate)
        {
            return _userDBService.Where(predicate);
        }

        public void LogOut()
        {
            if (SessionManager.Get<bool>(SessionState.IsLoggedOn))
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
                        { SessionState.LogInTime, DateTime.Now},
                        { SessionState.User, user }
                });
            }
            else
                throw new Exception(reason);

        }

        public string Register(User user)
        {

            string result = _userDBService.Insert(user);
            Token token = new Token
            {
                ExpirationDate = DateTime.Now.AddDays(7),
                IsDisabled = false,
                UserId = user.Id,
                Value = Guid.NewGuid(),
                Name = user.UserName + "'s Email Validtion Token",
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now
            };

            _tokenDBService.Insert(token);
            _emailService.Send("no-reply@nostreetssolutions.org", user.Contact.PrimaryEmail, "Nostreets Sandbox Validation", "", HttpContext.Current.Server.MapPath("\\assets\\ValidateEmail.html").ReadFile().FormatString("~/assets/ValidateEmail.html"));

            return result;
        }

        public bool ValidateEmail(Token token)
        {
            bool result = false;
            Token userToken = _tokenDBService.Where(
                a => !a.IsDisabled && a.ExpirationDate > DateTime.Now && a.Type == TokenType.EmailValidtion && a.UserId == token.UserId && a.Value == token.Value
            ).FirstOrDefault();

            if (userToken != null)
            {
                User user = Get(token.UserId);

                result = true;
                userToken.IsDisabled = true;
                userToken.DateModified = DateTime.Now;
                user.Settings.IsLockedOut = false;
                user.Settings.HasVaildatedEmail = true;

                if (user.Settings.IPAddresses == null)
                    user.Settings.IPAddresses = new List<string> { HttpContext.Current.GetRequestIPAddress() };
                else
                    user.Settings.IPAddresses.Add(HttpContext.Current.GetRequestIPAddress());

                _userDBService.Update(user);
                _tokenDBService.Update(userToken);
            }

            return result;
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
