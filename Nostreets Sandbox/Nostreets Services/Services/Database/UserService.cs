using Nostreets_Services.Domain;
using Nostreets_Services.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Nostreets_Services.Services.Database
{
    public class UserService : IUserService
    {
        public UserService()
        {
            _context = new UserDBContext("DefaultConnection");
        }

        public UserService(string connectionKey)
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
}
