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

        [Required, MinLength(6)]
        public string UserName { get; set; }

        [Required, MinLength(12)]
        public string Password { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastLogIn { get; set; }

        public UserSettings Settings { get; set; }

        public Contact Contact { get; set; }

    }

    public class UserSettings
    {

        private bool _isLockedOut = true;
        private bool _twoFactorAuthEnabled = false;
        private bool _hasVaildatedEmail = false;
        private bool _hasVaildatedPhone = false;
        private bool _hasPlaidSecret = false;
        private bool _TFAuthByEmail = false;
        private bool _TFAuthByPhone = false;


        public List<string> IPAddresses { get; set; }
        public bool TwoFactorAuthEnabled { get => _twoFactorAuthEnabled; set => _twoFactorAuthEnabled = value; }
        public bool TFAuthByEmail { get => _TFAuthByEmail; set => _TFAuthByEmail = value; }
        public bool TFAuthByPhone { get => _TFAuthByPhone; set => _TFAuthByPhone = value; }
        public bool IsLockedOut { get => _isLockedOut; set => _isLockedOut = value; }
        public bool HasVaildatedEmail { get => _hasVaildatedEmail; set => _hasVaildatedEmail = value; }
        public bool HasVaildatedPhone { get => _hasVaildatedPhone; set => _hasVaildatedPhone = value; }
        public bool HasPlaidSecret { get => _hasPlaidSecret; set => _hasPlaidSecret = value; }

    }

    public class Contact
    {
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string PrimaryEmail { get; set; }
        public string BackupEmail { get; set; }
        public string PrimaryPhone { get; set; }
        public string BackupPhone { get; set; }

    }
    
}