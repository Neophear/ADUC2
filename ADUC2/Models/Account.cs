using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ADUC2.Models
{
    public class Account
    {
        [RegularExpression(@"^[1-9]\d{5}$", ErrorMessage = "Skal være et validt MANR (100000 - 999999)")] //Must match an MANR (100000 > 999999)
        [Remote("ValidateAccountName", "Account")]
        public string AccountName { get; set; }

        [Remote("ValidatePassword", "Account", AdditionalFields = "AccountName")]
        public string Password { get; set; }

        public bool PasswordExpired { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string OU { get; set; }
        public DateTime? LastLogon { get; set; }
        public bool Locked { get; set; }
        public bool Enabled { get; set; }
        public DateTime? Expires { get; set; }
    }
}