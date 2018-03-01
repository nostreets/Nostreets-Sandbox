using Nostreets_Services.Domain;
using Nostreets_Services.Interfaces.Services;
using NostreetsExtensions;
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
            _userSrv = new DBService<User, string>("DefaultConnection");
        }

        public UserService(string connectionKey)
        {

            _userSrv = new DBService<User, string>(connectionKey);
        }

        DBService<User, string> _userSrv = null;

        public bool CheckIfUserExist(string username, string password)
        {
            string encryptedPassword = Encryption.SimpleDecryptWithPassword(password, password);
            return (_userSrv.Where(a => a.UserName == username) == null)
                        ? false
                   : _userSrv.Where(a => a.UserName == username && a.Password == encryptedPassword) != null
                        ? true
                        : false;
        }

        public void Delete(string id)
        {
            _userSrv.Delete(id);
        }

        public User Get(string id)
        {
            return _userSrv.Get(id);
        }

        public List<User> GetAll()
        {
            return _userSrv.GetAll();
        }

        public User GetByUsername(string username)
        {
            return _userSrv.Where(a => a.UserName == username).FirstOrDefault();
        }

        public string Insert(User model)
        {
            return _userSrv.Insert(model);
        }

        public void Update(User model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> Where(Func<User, bool> predicate)
        {
            return _userSrv.Where(predicate);
        }

        public void LogOut()
        {
            if (SessionManager.Get<bool>(SessionState.IsLoggedOn))
                SessionManager.AbandonSessions();
        }

        public void LogIn(string username, string password, bool rememberDevice = false)
        {
            User user = null;
            if (CheckIfUserExist(username, password))
            {
                user = GetByUsername(username);
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
                throw new Exception("User does not exist with inputted username and password...");

        }

        public bool Register(User user)
        {

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
