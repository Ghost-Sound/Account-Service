using AccountService.Application.Commands.Departments;
using AccountService.Domain.Entity;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CustomHelper.Exception;
using AccountService.Infrastructure.DB.Contexts;

namespace AccountService.Application.Handlers.Departmets
{
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, Department>
    {
        private readonly UserDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateDepartmentCommandHandler> _logger;

        public UpdateDepartmentCommandHandler(
            UserDbContext dbContext, 
            IMapper mapper, 
            ILogger<UpdateDepartmentCommandHandler> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Department> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingDepartment = await _dbContext.Set<Department>()
                    .Include(d => d.Users)
                    .FirstOrDefaultAsync(d => d.Id == request.UpdateDepartment.Id, cancellationToken);

                if (existingDepartment == null)
                {
                    throw new CustomException($"Department with Id {request.UpdateDepartment.Id} not found");
                }

                _mapper.Map(request.UpdateDepartment, existingDepartment);

                existingDepartment.LastModifiedDate = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync(cancellationToken);

                return existingDepartment;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error when updating Department: {ex.Message}");
                throw;
            }
        }
    }
}
