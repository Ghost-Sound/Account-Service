using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Publisher.Events
{
    public record DepartmentCreatedEvent
    {
        public Ulid Id { get; set; }
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
