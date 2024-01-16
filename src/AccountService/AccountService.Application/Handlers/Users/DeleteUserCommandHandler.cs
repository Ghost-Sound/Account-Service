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

namespace AccountService.Application.Handlers.Users
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly DbContext _dbContext;

        public DeleteUserCommandHandler(
            DbContext context)
        {
            _dbContext = context;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellation)
        {
            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(x => x.Id == request.UserDelete.Id, cancellation);

            if(user == null)
            {
                return false;
            }

            _dbContext.Set<User>().Remove(user);

            int affectedRows = await _dbContext.SaveChangesAsync(cancellation);

            return affectedRows > 0;
        }

    }
}
