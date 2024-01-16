using AccountService.Application.Commands.Users;
using AccountService.Application.Models;
using AccountService.Domain.Entity;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Handlers.Users
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDTO.UserUpdateDTO>
    {
        private readonly DbContext _dbContext;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(
            DbContext dbContext, 
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<UserDTO.UserUpdateDTO> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(x => x.Id == request.UserUpdate.Id, cancellationToken);

            if (user == null)
            {
                await Task.CompletedTask;
            }

            user = _mapper.Map<User>(user);

            _dbContext.Set<User>().Update(user);

            int affectedRows = await _dbContext.SaveChangesAsync(cancellationToken);

            if (affectedRows == 0)
            {
                await Task.CompletedTask;
            }

            return request.UserUpdate;
        }
    }
}
