using AccountService.Application.Models.Users;
using AccountService.Application.Queries.User;
using AccountService.Infrastructure.DB.Contexts;
using AutoMapper;
using CustomHelper.Helpers;
using MediatR;
using System.Data.Entity;

namespace AccountService.Application.Handlers.Users
{
    public class GetUsersByIdsQueryHandler(UserDbContext dbContext, IMapper mapper) : IRequestHandler<GetUsersByIdsQuery, List<UserGetDTO>>
    {
        public Task<List<UserGetDTO>> Handle(GetUsersByIdsQuery request, CancellationToken cancellationToken)
        {
            if(request.Ids == null)
            {
                return Task.FromResult(new List<UserGetDTO>());
            }

            if (request.Ids != null && request.Ids.Count == 0)
            {
                return Task.FromResult(new List<UserGetDTO>()); 
            }

            var users = mapper.Map<List<UserGetDTO>>(dbContext.Users
                .AsNoTracking()
                .Where(u => request.Ids.Select(id => id.ParseUlid()).Contains(u.Id))
                .ToList());

            return Task.FromResult(users);
        }
    }
}
