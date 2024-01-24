using AccountService.Application.Commands.Departments;
using AccountService.Application.Commands.Users;
using AccountService.Application.Models.Departments;
using AccountService.Application.Models.Users;
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
    public class CreateDepartmentHandler : IRequestHandler<CreateDepartmentCommand, CreateDepartmentDTO>
    {
        private readonly DbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public CreateDepartmentHandler(
            DbContext dbContext,
            IMapper mapper,
            ILogger logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CreateDepartmentDTO> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var department = _mapper.Map<Department>(request.CreateDepartment);

                if (department == null)
                {
                    _logger.LogError("Problem with creation department");
                    throw new CustomException("Problem with creation");
                }

                await _dbContext.Set<Department>().AddAsync(department, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return request.CreateDepartment;
            }
            catch
            {
                throw;
            }
        }
    }
}
