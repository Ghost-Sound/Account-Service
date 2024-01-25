using AccountService.Application.Commands.Departments;
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
    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, bool>
    {
        private readonly DbContext _dbContext;
        private readonly ILogger _logger;

        public DeleteDepartmentCommandHandler(
            DbContext dbContext, 
            ILogger logger) 
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task<bool> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var department = await _dbContext.Set<Department>().FirstOrDefaultAsync(x => x.Id == request.Ulid);

                if (department == null)
                {
                    _logger.LogError($"Object is null {nameof(department)}");

                    throw new ArgumentNullException(nameof(department));
                }

                _dbContext.Set<Department>().Remove(department);
                int affectedRows = await _dbContext.SaveChangesAsync(cancellationToken);

                if (affectedRows == 0)
                {
                    throw new DataNotModifiedException();
                }
                _logger.LogInformation("removal was successful");

                return affectedRows > 0;
            }
            catch
            {
                throw;
            }
        }
    }
}
