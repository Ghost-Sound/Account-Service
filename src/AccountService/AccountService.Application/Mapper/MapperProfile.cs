using AccountService.Application.Models.Departments;
using AccountService.Application.Models.Users;
using AccountService.Domain.Entity;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            #region User Map
            CreateMap<UserRegistryDTO, User>();
            CreateMap<UserGetDTO, User>();
            CreateMap<User, UserGetDTO>();
            CreateMap<UserUpdateDTO, User>();
            #endregion

            #region Department Map
            CreateMap<CreateDepartmentDTO, Department>();
            CreateMap<GetDepartmentDTO, Department>();
            CreateMap<UpdateDepartmentDTO, Department>();
            CreateMap<Department, GetDepartmentDTO>();
            #endregion
        }
    }
}
