using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Publisher.Events
{
    public record UserCreatedEvent
    {
        public Ulid Id { get; set; }
        public DateTime Created { get; set; }
    }
}
