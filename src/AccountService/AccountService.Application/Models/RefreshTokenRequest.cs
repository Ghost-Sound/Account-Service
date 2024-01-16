using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Models
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
