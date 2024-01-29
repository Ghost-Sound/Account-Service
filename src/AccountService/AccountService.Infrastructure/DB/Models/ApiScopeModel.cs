using System.ComponentModel.DataAnnotations;

namespace AccountService.Infrastructure.DB.Models
{
    public class ApiScopeSummaryModel
    {
        [Required]
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }

    public class ApiScopeModel : ApiScopeSummaryModel
    {
        public string UserClaims { get; set; }
    }
}
