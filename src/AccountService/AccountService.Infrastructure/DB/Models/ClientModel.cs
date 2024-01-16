using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Infrastructure.DB.Models
{
    public enum Flow
    {
        ClientCredentials,
        CodeFlowWithPkce
    }

    public class ClientSummaryModel
    {
        [Required]
        public string ClientId { get; set; }
        public string Name { get; set; }
        [Required]
        public Flow Flow { get; set; }
    }


    public class CreateClientModel : ClientSummaryModel
    {
        public string Secret { get; set; }
    }

    public class ClientModel : CreateClientModel, IValidatableObject
    {
        [Required]
        public string AllowedScopes { get; set; }

        public string RedirectUri { get; set; }
        public string PostLogoutRedirectUri { get; set; }
        public string FrontChannelLogoutUri { get; set; }
        public string BackChannelLogoutUri { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            if (Flow == Flow.CodeFlowWithPkce)
            {
                if (RedirectUri == null)
                {
                    errors.Add(new ValidationResult("Redirect URI is required.", new[] { "RedirectUri" }));
                }
            }

            return errors;
        }
    }

    
}
