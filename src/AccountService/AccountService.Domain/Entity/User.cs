using Microsoft.AspNetCore.Identity;

namespace AccountService.Domain.Entity
{
    public class User : IdentityUser<Ulid>
    {
        public User()
        {
            Departments = new List<Department>();
        }
        public override Ulid Id { get => base.Id; set => base.Id = Ulid.NewUlid(); }

        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }

        public DateTime LastSuccessfullEmailVerification { get; set; }
        public DateTime LastSuccessfullLogin { get; set; }

        #region Relation Ship
        public ICollection<Department> Departments { get; set; }
        #endregion
    }
}
