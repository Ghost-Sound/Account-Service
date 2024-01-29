﻿using AccountService.Application.Models.Departments;
using AccountService.Application.Queries.Departments;
using AccountService.Domain.Entity;
using AccountService.Infrastructure.DB.Contexts;
using AutoMapper;
using CustomHelper.Exception;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AccountService.Application.Handlers.Departmets
{
    public class GetDepartmentQueryHandler : IRequestHandler<GetDepartmentQuery, GetDepartmentDTO>
    {
        private readonly UserDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<GetDepartmentQueryHandler> _logger;

        public GetDepartmentQueryHandler(
            UserDbContext dbContext,
            IMapper mapper,
            ILogger<GetDepartmentQueryHandler> logger)
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
