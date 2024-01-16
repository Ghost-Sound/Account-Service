using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Infrastructure.DB.Models
{
    public class ThirdPartyInitiatedLoginLink
    {
        public string LinkText { get; set; }
        public string InitiateLoginUri { get; set; }
    }
}
