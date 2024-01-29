using AccountService.Application.Commands.Users;
using AccountService.Domain.Entity;
using AccountService.Infrastructure.DB.Contexts;
using AutoMapper;
using CustomHelper.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Application.Handlers.Users
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, User>
    {
        private readonly UserDbContext _dbContext;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(
            UserDbContext dbContext, 
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<User> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _dbContext.Set<User>().FirstOrDefaultAsync(x => x.Id == request.UserUpdate.Id, cancellationToken);

                if (user == null)
                {
                    throw new CustomException("Not found user");
                }

                user = _mapper.Map<User>(request.UserUpdate);

                _dbContext.Set<User>().Update(user);

                int affectedRows = await _dbContext.SaveChangesAsync(cancellationToken);

                if (affectedRows == 0)
                {
                    throw new DataNotModifiedException();
                }

                return user;
            }
            catch
            {
                throw;
            }
            
        }
    }
}
