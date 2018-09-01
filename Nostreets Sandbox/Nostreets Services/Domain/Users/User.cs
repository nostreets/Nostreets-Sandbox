using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Nostreets_Services.Enums;
using NostreetsExtensions.DataControl.Classes;

namespace Nostreets_Services.Domain
{

    public class User
    {
        [Key]
        public string Id { get; set; }

        [Required, MinLength(6)]
        public string UserName { get; set; }

        [Required, MinLength(12)]
        public string Password { get; set; }

        public DateTime? DateCreated { get; set; } = DateTime.Now;

        public DateTime? DateModified { get; set; } = DateTime.Now;

        public DateTime? LastLogIn { get; set; }

        public UserSettings Settings { get; set; }

        [Required]
        public Contact Contact { get; set; }

        [Required]
        public UserOriginType UserOrigin { get; set; }

    }

    public class UserSettings : DBObject
    {
        public List<string> IPAddresses { get; set; }
        public NostreetsPortfolioSettings Portfolio { get; set; }

        public bool TwoFactorAuthEnabled { get; set; } = false;
        public bool TFAuthByEmail { get; set; } = false;
        public bool TFAuthByPhone { get; set; } = false;
        public bool IsLockedOut { get; set; } = true;
        public bool HasVaildatedEmail { get; set; } = false;
        public bool HasVaildatedPhone { get; set; } = false;
        public bool ValidateIPBeforeLogin { get; set; } = false;
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

    public class NostreetsPortfolioSettings
    {
        public bool HasPlaidSecret { get; set; } = false;
        public bool IsAdvancedUser { get; set; } = false;
        public ChartLibraryType ChartLibary { get; set; } = ChartLibraryType.Chartist;

    }

}