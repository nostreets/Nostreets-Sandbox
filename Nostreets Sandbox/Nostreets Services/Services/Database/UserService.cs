using Nostreets_Services.Domain;
using Nostreets_Services.Domain.Base;
using Nostreets_Services.Interfaces.Services;
using NostreetsExtensions;
using NostreetsExtensions.Interfaces;
using NostreetsExtensions.Utilities;
using NostreetsORM;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Nostreets_Services.Services.Database
{
    public class UserService : IUserService
    {
        public UserService()
        {
            UserDBService = new DBService<User, string>("DefaultConnection");
        }

        public UserService(string connectionKey)
        {
            UserDBService = new DBService<User, string>(connectionKey);
        }

        [Microsoft.Practices.Unity.Dependency]
        private IEmailService EmailService { get; set; }
        [Microsoft.Practices.Unity.Dependency]
        private IDBService<User, string> UserDBService { get; set; }
        [Microsoft.Practices.Unity.Dependency]
        private IDBService<Token, string> TokenDBService { get; set; }

        public bool CheckIfUserCanLogIn(string username, string password, out string failureReason)
        {
            failureReason = null;
            bool result = false;
            string encryptedPassword = Encryption.SimpleEncryptWithPassword(password, password);
            User user = UserDBService.Where(a => a.UserName == username).FirstOrDefault();
            if (user == null)
                failureReason = "User doesn't exist...";

            else if (user.Password != encryptedPassword)
                failureReason = "Invalid password for " + username + "...";

            else if (!user.Settings.HasVaildatedEmail)
                failureReason = "Email is not validated...";

            else if (user.Settings.TwoFactorAuthEnabled)
            {
                //TODO
                failureReason = "2nd Code was sent to " + ((user.Settings.TFAuthByPhone) ? "Phone" : "Email");
            }
            else
                result = true;

            return result;
        }

        public void Delete(string id)
        {
            UserDBService.Delete(id);
        }

        public User Get(string id)
        {
            return UserDBService.Get(id);
        }

        public List<User> GetAll()
        {
            return UserDBService.GetAll();
        }

        public User GetByUsername(string username)
        {
            return UserDBService.Where(a => a.UserName == username).FirstOrDefault();
        }

        public string Insert(User model)
        {
            return UserDBService.Insert(model);
        }

        public void Update(User model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> Where(Func<User, bool> predicate)
        {
            return UserDBService.Where(predicate);
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

                if (user.Settings.IPAddresses == null)


                    if (rememberDevice && (user.Settings.IPAddresses == null || !user.Settings.IPAddresses.Contains(new Tuple<string, string, string>(HttpContext.Current.GetRequestIPAddress(), username, password))))
                        user.Settings.IPAddresses.Add(new Tuple<string, string, string>(HttpContext.Current.GetRequestIPAddress(), username, password));

                SessionManager.Add(new Dictionary<SessionState, object>{
                        { SessionState.IsLoggedOn, true},
                        { SessionState.IsUser, true},
                        { SessionState.LogOffTime, DateTime.Now.AddMinutes(30)},
                        { SessionState.LogInTime, DateTime.Now},
                        { SessionState.LogOffTime, user }
                });
            }
            else
                throw new Exception(reason);

        }

        public string Register(User user)
        {

            string result = UserDBService.Insert(user);
            Token token = new Token
            {
                ExpirationDate = DateTime.Now.AddDays(7),
                IsDisabled = false,
                UserId = user.Id,
                Value = Guid.NewGuid()
            };

            TokenDBService.Insert(token);
            EmailService.Send("no-reply@nostreetssolutions.org", "Nostreets Solutions", user.Contact.PrimaryEmail, "Nostreets Sandbox Validation", "", HttpContext.Current.Server.MapPath("\\assets\\ValidateEmail.html").ReadFile());

            return result;
        }

        public bool ValidateEmail(Token token)
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
