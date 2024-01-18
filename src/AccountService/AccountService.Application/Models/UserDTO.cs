using AccountService.Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Models
{
    public class UserDTO
    {
        public class UserRegistryDTO
        {
            [Required]
            public string Username { get; set; }

            [Required]
            public string Password { get; set; }

            public string? FirstName { get; set; }
            public string? MiddleName { get; set; }
            public string? LastName { get; set; }
            [Phone]
            public virtual string? PhoneNumber { get; set; }

            [EmailAddress]
            public string Email { get; set; }

            public string ReturnUrl { get; set; }
        }

        public class UserLoginDTO
        {
                [Required]
                public string Username { get; set; }

                [Required]
                public string Password { get; set; }

                public bool RememberLogin { get; set; }

            [EmailAddress]
            public string Email { get; set; }

            public string ReturnUrl { get; set; }
        }

        public class UserLogOutModel
        {
            public string PostLogoutRedirectUri { get; set; }
            public string ClientName { get; set; }
            public string SignOutIframeUrl { get; set; }
            public bool AutomaticRedirectAfterSignOut { get; set; }
        }

        public class UserDeleteDTO
        {
            [Required]
            public Ulid Id { get; set; }
        }

        public class UserUpdateDTO
        {
            public Ulid Id { get; set; }

            public string? FirstName { get; set; }
            public string? MiddleName { get; set; }
            public string? LastName { get; set; }

            public DateTime LastSuccessfullEmailVerification { get; set; }
            public DateTime LastSuccessfullLogin { get; set; }

            #region Relation Ship
            public List<Ulid> Groups { get; set; }
            #endregion
        }

        public class UserGetDTO
        {
            public Ulid Id { get; set; }

            public string? FirstName { get; set; }
            public string? MiddleName { get; set; }
            public string? LastName { get; set; }

            public DateTime LastSuccessfullEmailVerification { get; set; }
            public DateTime LastSuccessfullLogin { get; set; }

            #region Relation Ship
            public List<Ulid> Groups { get; set; }
            #endregion
        }

        public class UserGetByIdDTO
        {
            [Required]
            public Ulid Id { get; set; }
        }

        public class UsersGetDTO
        {
            public List<SortParameter> SortParameters { get; set; } = new List<SortParameter>();

            public int PageSize {  get; set; }
            public int Page {  get; set; }

            public bool SortDescending {  get; set; } = true;
        }
    }
}
