using AccountService.Application.Commands.Users;
using AccountService.Domain.Entity;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AccountService.Application.Models.UserDTO;

namespace AccountService.Application.Handlers.Users
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserRegistryDTO>
    {
        private readonly DbContext _dbContext;
        private readonly IMapper _mapper;
        public CreateUserCommandHandler(DbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<UserRegistryDTO> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<User>(request.User);
            await _dbContext.Set<User>().AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var userDto = _mapper.Map<UserRegistryDTO>(user);
            return userDto;
        }
    }
}
