using AccountService.Application.Commands.Departments;
using AccountService.Application.Models.Departments;
using AccountService.Domain.Entity;
using AccountService.Infrastructure.DB.Contexts;
using AccountService.Publisher.Events;
using AutoMapper;
using CustomHelper.Exception;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;

namespace AccountService.Application.Handlers.Departmets
{
    public class CreateDepartmentHandler : IRequestHandler<CreateDepartmentCommand, GetDepartmentDTO>
    {
        private readonly UserDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateDepartmentHandler> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateDepartmentHandler(
            UserDbContext dbContext,
            IMapper mapper,
            ILogger<CreateDepartmentHandler> logger,
            IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<GetDepartmentDTO> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var department = _mapper.Map<Department>(request.CreateDepartment);

                var users = _dbContext.Users.Where(u => request.CreateDepartment.Users!.Contains(u.Id));

                department.Users = users.ToList();

                if (department == null)
                {
                    _logger.LogError("Problem with creation department");
                    throw new CustomException("Problem with creation");
                }

                await _dbContext.Set<Department>().AddAsync(department, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                await _publishEndpoint.Publish(new DepartmentCreatedEvent
                {
                    Id = department.Id,
                    Created = DateTime.UtcNow,
                    Description = department.Description,
                    Email = department.Email,
                    Name = department.Name,
                    PhoneNumber = department.PhoneNumber,
                }, cancellationToken);

                var dto = _mapper.Map<GetDepartmentDTO>(_dbContext.Set<Department>().FirstOrDefaultAsync(x => x.Id == department.Id, cancellationToken));


                return dto; 
            }
            catch
            {
                throw;
            }
        }
    }
}
