using AccountService.Application.Models.Users;
using AccountService.Application.Queries.User;
using AccountService.Domain.Entity;
using AccountService.Infrastructure.DB.Contexts;
using AutoMapper;
using CustomHelper.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Application.Handlers.Users
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserGetDTO>>
    {
        private readonly UserDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(
            UserDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }


        public async Task<IEnumerable<UserGetDTO>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            try
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

                var userDTOs = _mapper.Map<IEnumerable<UserGetDTO>>(users);

                return userDTOs;
            }
            catch
            {
                throw;
            }
            
        }
    }
}
