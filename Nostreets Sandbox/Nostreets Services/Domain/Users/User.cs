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
        DateTime _dateCreated = DateTime.Now;
        DateTime _dateModified = DateTime.Now;

        [Key]
        public string Id { get; set; }

        [Required, MinLength(6)]
        public string UserName { get; set; }

        [Required, MinLength(12)]
        public string Password { get; set; }

        public DateTime? DateCreated { get => _dateCreated; set=> _dateCreated = value.Value; }

        public DateTime? DateModified { get => _dateModified; set => _dateModified = value.Value; }

        public DateTime? LastLogIn { get; set; }

        public UserSettings Settings { get; set; }

        public Contact Contact { get; set; }

    }

    public class UserSettings : DBObject
    {
        private bool _isLockedOut = true;
        private bool _twoFactorAuthEnabled = false;
        private bool _hasVaildatedEmail = false;
        private bool _hasVaildatedPhone = false;
        private bool _hasPlaidSecret = false;
        private bool _TFAuthByEmail = false;
        private bool _TFAuthByPhone = false;
        private bool _validateIPBeforeLogin = false;


        public List<string> IPAddresses { get; set; }
        public bool TwoFactorAuthEnabled { get => _twoFactorAuthEnabled; set => _twoFactorAuthEnabled = value; }
        public bool TFAuthByEmail { get => _TFAuthByEmail; set => _TFAuthByEmail = value; }
        public bool TFAuthByPhone { get => _TFAuthByPhone; set => _TFAuthByPhone = value; }
        public bool IsLockedOut { get => _isLockedOut; set => _isLockedOut = value; }
        public bool HasVaildatedEmail { get => _hasVaildatedEmail; set => _hasVaildatedEmail = value; }
        public bool HasVaildatedPhone { get => _hasVaildatedPhone; set => _hasVaildatedPhone = value; }
        public bool HasPlaidSecret { get => _hasPlaidSecret; set => _hasPlaidSecret = value; }
        public bool ValidateIPBeforeLogin { get => _validateIPBeforeLogin; set => _validateIPBeforeLogin = value; }

    }

    public class Contact : DBObject
    {
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required]
        public string PrimaryEmail { get; set; }
        public string PrimaryPhone { get; set; }

    }

    public class NamePasswordPair
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }


}