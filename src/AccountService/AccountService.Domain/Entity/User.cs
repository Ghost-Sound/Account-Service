using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Domain.Entity
{
    public class User : IdentityUser<Ulid>
    {
        public User()
        {
            Groups = new List<GroupEntity>();
            ProfilePicture = new List<ProfilePicture>();
        }
        public override Ulid Id { get => base.Id; set => base.Id = Ulid.NewUlid(); }

        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }

        public DateTime LastSuccessfullEmailVerification { get; set; }
        public DateTime LastSuccessfullLogin { get; set; }

        #region Relation Ship
        public ICollection<GroupEntity> Groups { get; set; }

        public ICollection<ProfilePicture> ProfilePicture { get; set; }
        #endregion
    }
}
