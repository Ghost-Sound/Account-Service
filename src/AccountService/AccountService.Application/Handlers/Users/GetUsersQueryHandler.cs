using AccountService.Application.Models;
using AccountService.Application.Queries;
using AccountService.Domain.Entity;
using AutoMapper;
using CustomHelper.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AccountService.Application.Handlers.Users
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserDTO.UserGetDTO>>
    {
        private readonly DbContext _dbContext;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(
            DbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }


        public async Task<IEnumerable<UserDTO.UserGetDTO>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Set<User>().AsNoTracking();

            if (request.Users.SortParameters.Any())
            {
                var convertedSortParameters = request.Users.SortParameters
                                            .Select(sp => (sp.PropertyName, sp.IsDescending))
                                            .ToList();

                query = query.OrderByDynamic(convertedSortParameters);
            }

            if (request.Users.Page > 0 && request.Users.PageSize > 0)
            {
                query = query
                    .Skip((request.Users.Page - 1) * request.Users.PageSize)
                    .Take(request.Users.PageSize);
            }

            var users = await query.ToListAsync(cancellationToken);

            var userDTOs = _mapper.Map<IEnumerable<UserDTO.UserGetDTO>>(users);

            return userDTOs;
        }
    }
}
