using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Models.Users
{
    public class UserUpdateDTO
    {
        public Ulid Id { get; set; }

        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }

        public DateTime? LastSuccessfullEmailVerification { get; set; }
        public DateTime? LastSuccessfullLogin { get; set; }

        #region Relation Ship
        public List<Ulid> Groups { get; set; }
        #endregion
    }
}
