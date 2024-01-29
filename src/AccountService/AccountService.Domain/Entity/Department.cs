﻿using System.ComponentModel.DataAnnotations;

namespace AccountService.Domain.Entity
{
    public class Department
    {
        public Department()
        {
            Id = Ulid.NewUlid();
            Users = new List<User>();
        }

        public Ulid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        #region Relation Ship
        public ICollection<User> Users { get; set; }
        #endregion
    }
}
