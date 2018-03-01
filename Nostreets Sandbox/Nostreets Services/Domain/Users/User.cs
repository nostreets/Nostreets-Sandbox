using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using Nostreets_Services.Domain.Base;
using System.Collections.Generic;

namespace Nostreets_Services.Domain
{
    public class User
    {
        [Key, Required]
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastLogIn { get; set; }

        public UserSettings Settings { get; set; }

        public Contact Contact { get; set; }

    }

    public class UserSettings
    {
        public UserSettings()
        {
            TwoFactorAuthEnabled = false;
            IsLockedOut = true;
            HasVaildatedEmail = false;
            HasVaildatedPhone = false;
            HasPlaidSecret = false;
        }

        public bool TwoFactorAuthEnabled { get; set; }
        public bool IsLockedOut { get; set; }
        public bool HasVaildatedEmail { get; set; }
        public bool HasVaildatedPhone { get; set; }
        public bool HasPlaidSecret { get; set; }
        public List<Tuple<string, string, string>> IPAddresses { get; set; }


    }

    public class Contact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PrimaryEmail { get; set; }
        public string BackupEmail { get; set; }
        public string PrimaryPhone { get; set; }
        public string BackupPhone { get; set; }

    }
    
}