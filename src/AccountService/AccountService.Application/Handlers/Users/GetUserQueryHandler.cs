using AccountService.Application.Models;
using AccountService.Application.Queries;
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
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDTO.UserGetDTO>
    {
        private readonly DbContext _dbContext;
        private readonly IMapper _mapper;

        public GetUserQueryHandler(
            DbContext dbContext, 
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<UserDTO.UserGetDTO> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Set<User>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (user == null)
            {
                await Task.CompletedTask;
            }

            var dto = _mapper.Map<UserDTO.UserGetDTO>(user);

            return dto;
        }
    }
}
