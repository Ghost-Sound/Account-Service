using System.ComponentModel.DataAnnotations;

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
