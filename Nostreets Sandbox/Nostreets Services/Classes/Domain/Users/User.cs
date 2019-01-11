using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nostreets_Services.Enums;
using NostreetsExtensions.DataControl.Classes;
using NostreetsExtensions.DataControl.Enums;

namespace Nostreets_Services.Classes.Domain.Users
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

        public StreetAddress Address { get; set; }

    }

    public class NamePasswordPair 
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class NostreetsPortfolioSettings : DBObject
    {
        public bool HasPlaidSecret { get; set; } = false;
        public bool IsAdvancedUser { get; set; } = false;
        public ChartLibraryType ChartLibary { get; set; } = ChartLibraryType.Google;

    }

    public class StreetAddress : DBObject
    {
        [Required]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string OwnersFirstName { get; set; }

        [Required]
        public string OwnersLastName { get; set; }

        public string PhoneNumber { get; set; }

        public string State { get; set; }

        public int ZipCode { get; set; }

    }

    public class UserData : DBObject
    {
        public Dictionary<string, string> ApiIDs { get; set; }
        public List<string> IPAddresses { get; set; }
    }

    public class LogInResponse {

        public State State { get; set; }
        public string Message { get; set; }
        public User User { get; set; }
    }
}