using CustomHelper.EFcore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Domain.Entity
{
    public class GroupEntity : BaseChange<User>
    {
        public GroupEntity()
        {
            Id = Ulid.NewUlid();
            Users = new List<User>();
        }
        public Ulid Id { get; set; }
        public Ulid DepartmentId { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public override DateTime? CreationDate { get; set; }

        public override DateTime? LastModifiedDate { get; set; }

        public override User? ModifiedUser { get; set; }

        #region Relation Ship
        public ICollection<User> Users { get; set; }

        public Department Department { get; set; }
        #endregion
    }
}
