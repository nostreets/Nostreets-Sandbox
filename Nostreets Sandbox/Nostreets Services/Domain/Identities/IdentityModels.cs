using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace Nostreets_Services.Domain
{
    public class User : IUser
    {
        [Key]
        public string Id { get; set; }

        public string UserName { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            return userIdentity;
        }
    }

    public class UserDbContext : DbContext
    {
        public UserDbContext()
            : base("DefaultConnection" /*"AzureDBConnection"*/)
        {  }

        public IDbSet<User> Users { get; set; }
    }
}