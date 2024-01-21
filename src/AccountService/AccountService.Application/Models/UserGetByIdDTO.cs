using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Models
{
    public class UserGetByIdDTO
    {
        [Required]
        public Ulid Id { get; set; }
    }
}
