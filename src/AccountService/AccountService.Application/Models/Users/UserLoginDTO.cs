using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Models.Users
{
    public class UserLoginDTO
    {
        [Required]
        public string Password { get; set; } = null!;

        public bool RememberLogin { get; set; } = false;

        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
