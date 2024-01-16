using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Models
{
    public class DeviceDTO
    {
        public class DeviceInputDTO
        {
            public string Button { get; set; }
            public IEnumerable<string> ScopesConsented { get; set; }
            public bool RememberConsent { get; set; } = true;
            public string ReturnUrl { get; set; }
            public string Description { get; set; }
            public string UserCode { get; set; }
        }

        public class ScopeViewModel
        {
            public string Value { get; set; }
            public string DisplayName { get; set; }
            public string Description { get; set; }
            public bool Emphasize { get; set; }
            public bool Required { get; set; }
            public bool Checked { get; set; }
        }

        public class DeviceViewDTO
        {
            public string ClientName { get; set; }
            public string ClientUrl { get; set; }
            public string ClientLogoUrl { get; set; }
            public bool AllowRememberConsent { get; set; }

            public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }
            public IEnumerable<ScopeViewModel> ApiScopes { get; set; }
        }
    }
}
