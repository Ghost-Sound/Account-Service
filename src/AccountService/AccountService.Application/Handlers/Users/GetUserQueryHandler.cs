using AccountService.Application.Models;
using AccountService.Application.Queries;
using AccountService.Domain.Entity;
using AutoMapper;
using CustomHelper.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Handlers.Users
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserGetDTO>
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

        public async Task<UserGetDTO> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _dbContext.Set<User>()
               .AsNoTracking()
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
