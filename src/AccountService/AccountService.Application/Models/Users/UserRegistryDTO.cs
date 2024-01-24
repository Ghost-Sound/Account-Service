using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Models.Users
{
    public class UserRegistryDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        [Phone]
        public virtual string? PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string ReturnUrl { get; set; }
    }
}
