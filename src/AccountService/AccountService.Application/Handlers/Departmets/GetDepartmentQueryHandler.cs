using AccountService.Application.Models.Departments;
using AccountService.Application.Models.Users;
using AccountService.Application.Queries.Departments;
using AccountService.Application.Queries.User;
using AccountService.Domain.Entity;
using AutoMapper;
using CustomHelper.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.Handlers.Departmets
{
    public class GetDepartmentQueryHandler : IRequestHandler<GetDepartmentQuery, GetDepartmentDTO>
    {
        private readonly DbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetDepartmentQueryHandler(
            DbContext dbContext,
            IMapper mapper,
            ILogger logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GetDepartmentDTO> Handle(GetDepartmentQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var existingDepartment = await _dbContext.Set<Department>()
                    .AsNoTracking()
                    .Include(d => d.Users)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);

                if (existingDepartment == null)
                {
                    throw new CustomException($"Department with Id {request.Id} not found");
                }

                var dto = _mapper.Map<GetDepartmentDTO>(existingDepartment);

                return dto;
            }
            catch
            {
                throw;
            }
        }
    }
}
