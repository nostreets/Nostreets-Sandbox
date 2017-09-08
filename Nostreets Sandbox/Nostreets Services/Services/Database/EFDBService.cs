using NostreetsORM.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Services.Database
{
    class EFDBService<T> : IDBService<T> where T : class
    {

        public EFDBService()
        {
            _context = new EFDbContext<T>();
        }
        public EFDBService(string connectionKey)
        {
            _context = new EFDbContext<T>(connectionKey, typeof(T).Name);
        }

        EFDbContext<T> _context = null;

        public List<T> GetAll()
        {
            return _context.Table.ToList();
        }

        public T Get(object id)
        {
            T user = _context.Table.FirstOrDefault(a => a.GetType().GetProperties().GetValue(0) == id);
            return user;
        }

        public object Insert(T model)
        {
            var firstPTypeName = model.GetType().GetProperties()[0].GetType().Name;

            if (firstPTypeName.Contains("INT"))
            {
                model.GetType().GetProperties().SetValue(GetAll().Count + 1, 0);
            }
            else if (firstPTypeName == "GUID")
            {
                model.GetType().GetProperties().SetValue(Guid.NewGuid().ToString(), 0);
            }
            _context.Table.Add(model);

            if (_context.SaveChanges() == 0) { throw new Exception("DB changes not saved!"); }

            return model.GetType().GetProperties().GetValue(0);
        }

        public void Delete(object id)
        {
            T obj = _context.Table.FirstOrDefault(a => a.GetType().GetProperties().GetValue(0) == id);

            _context.Table.Remove(obj);

            if (_context.SaveChanges() == 0) { throw new Exception("DB changes not saved!"); }
        }

        public void Update(object model)
        {
            T targetedUser = _context.Table.FirstOrDefault(a => a.GetType().GetProperties().GetValue(0) == model.GetType().GetProperties().GetValue(0));
            targetedUser = (T)model;

            if (_context.SaveChanges() == 0) { throw new Exception("DB changes not saved!"); }
        }

        public IEnumerable<T> Where() { }

        private class EFDbContext<TContext> : DbContext where TContext : class
        {
            public EFDbContext()
                : base("DefaultConnection")
            { }

            public EFDbContext(string connectionKey)
                : base(connectionKey)
            { }

            public EFDbContext(string connectionKey, string tableName)
                : base(connectionKey)
            {
                OnModelCreating(new DbModelBuilder().HasDefaultSchema(tableName));
            }

            public IDbSet<TContext> Table { get; set; }

        }

    }
}
