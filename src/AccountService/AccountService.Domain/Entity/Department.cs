using CustomHelper.EFcore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Domain.Entity
{
    public class Department : BaseChange<User>
    {
        public Department()
        {
            Id = Ulid.NewUlid();
            Groups = new List<GroupEntity>();
        }

        public Ulid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public override DateTime? CreationDate { get; set; }

        public override DateTime? LastModifiedDate { get; set; }

        public override User? ModifiedUser { get; set; }

        #region Relation Ship
        public ICollection<GroupEntity> Groups { get; set; }
        #endregion
    }
}
