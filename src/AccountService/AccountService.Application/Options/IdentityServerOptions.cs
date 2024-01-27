using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Options
{
    public class IdentityServerOptions
    {
        public string URL { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string SigninPath {  get; set; }
        public string LogOutPath { get; set; }
        public string PostLogOutRedirect {  get; set; }
    }
}
