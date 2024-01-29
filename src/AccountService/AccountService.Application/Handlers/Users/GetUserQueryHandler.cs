using AccountService.Application.Models.Users;
using AccountService.Application.Queries.User;
using AccountService.Domain.Entity;
using AccountService.Infrastructure.DB.Contexts;
using AutoMapper;
using CustomHelper.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Application.Handlers.Users
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserGetDTO>
    {
        private readonly UserDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetUserQueryHandler(
            UserDbContext dbContext, 
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<UserGetDTO> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _dbContext.Set<User>()
               .AsNoTracking()
               .Include(u => u.Departments)
               .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (user == null)
                {
                    throw new CustomException("Not found user");
                }

                var dto = _mapper.Map<UserGetDTO>(user);

                return dto;
            }
            catch
            {
                throw;
            }  
        }
    }
}
