using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Infrastructure.DB.Models
{
    public class IdentityScopeSummaryModel
    {
        [Required]
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }

    public class IdentityScopeModel : IdentityScopeSummaryModel
    {
        public string UserClaims { get; set; }
    }
}
