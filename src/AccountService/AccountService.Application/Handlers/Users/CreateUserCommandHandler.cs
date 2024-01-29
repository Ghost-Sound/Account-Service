using AccountService.Application.Commands.Users;
using AccountService.Application.Models.Users;
using AccountService.Domain.Entity;
using AccountService.Infrastructure.DB.Contexts;
using AutoMapper;
using MediatR;

namespace AccountService.Application.Handlers.Users
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserRegistryDTO>
    {
        private readonly UserDbContext _dbContext;
        private readonly IMapper _mapper;
        public CreateUserCommandHandler(
            UserDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<UserRegistryDTO> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = _mapper.Map<User>(request.User);
                await _dbContext.Set<User>().AddAsync(user, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                var userDto = _mapper.Map<UserRegistryDTO>(user);
                return userDto;
            }
            catch
            {
                throw;
            }
        }
    }
}
