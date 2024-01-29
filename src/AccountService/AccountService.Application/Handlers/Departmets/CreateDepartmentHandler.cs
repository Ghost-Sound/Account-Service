using AccountService.Application.Commands.Departments;
using AccountService.Application.Models.Departments;
using AccountService.Domain.Entity;
using AccountService.Infrastructure.DB.Contexts;
using AutoMapper;
using CustomHelper.Exception;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AccountService.Application.Handlers.Departmets
{
    public class CreateDepartmentHandler : IRequestHandler<CreateDepartmentCommand, CreateDepartmentDTO>
    {
        private readonly UserDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateDepartmentHandler> _logger;
        public CreateDepartmentHandler(
            UserDbContext dbContext,
            IMapper mapper,
            ILogger<CreateDepartmentHandler> logger)
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
