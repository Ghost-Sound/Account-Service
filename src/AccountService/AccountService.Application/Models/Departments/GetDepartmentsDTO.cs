using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Models.Departments
{
    public class GetDepartmentsDTO
    {
        public List<SortParameter> SortParameters { get; set; } = new List<SortParameter>();

        public int PageSize { get; set; }
        public int Page { get; set; }
    }
}
