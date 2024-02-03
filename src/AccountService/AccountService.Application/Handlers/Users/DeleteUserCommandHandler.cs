﻿using AccountService.Application.Commands.Users;
using AccountService.Domain.Entity;
using AccountService.Infrastructure.DB.Contexts;
using CustomHelper.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Application.Handlers.Users
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly UserDbContext _dbContext;

        public DeleteUserCommandHandler(
            UserDbContext context)
        {
            _dbContext = context;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellation)
        {
            try
            {
                var user = await _dbContext.Set<User>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellation);

                if (user == null)
                {
                    return false;
                }

                _dbContext.Set<User>().Remove(user);

                int affectedRows = await _dbContext.SaveChangesAsync(cancellation);

                if(affectedRows == 0)
                {
                    throw new DataNotModifiedException();
                }

                return affectedRows > 0;
            }
            catch
            {
                throw;
            }
        }

    }
}
