using System.ComponentModel.DataAnnotations;

namespace AccountService.Application.Models.Departments
{
    public class GetDepartmentDTO
    {
        public Ulid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public List<Ulid> Users { get; set; }
        public DateTime? CreationDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }
    }
}
