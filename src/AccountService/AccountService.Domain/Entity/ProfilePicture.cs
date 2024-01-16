using CustomHelper.EFcore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Domain.Entity
{
    public class ProfilePicture : BaseChange<User>
    {
        public ProfilePicture()
        {
            Id = Ulid.NewUlid();
        }
        public Ulid Id { get; set; }

        public Ulid UserId { get; set; }

        public string Name { get; set; }
        public string? ProfilePictureData { get; set; }
        public double? ProfilePictureSize { get; set; }

        public override DateTime? CreationDate { get; set; }

        public override DateTime? LastModifiedDate { get; set; }

        public override User? ModifiedUser { get; set; }

        #region Relation Ship
        public User User { get; set; }
        #endregion

    }
}
