using AccountService.Domain.Entity;
using AccountService.Infrastructure.DB.Contexts;
using AccountService.Publisher.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Publisher.Consumers
{
    public sealed class DepartmentCreatedConsumer : IConsumer<DepartmentCreatedEvent>
    {
        private readonly UserDbContext _userDbContext;
        private readonly ILogger<DepartmentCreatedConsumer> _logger;

        public DepartmentCreatedConsumer(UserDbContext userDbContext, 
            ILogger<DepartmentCreatedConsumer> logger)
        {
            _userDbContext = userDbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<DepartmentCreatedEvent> context)
        {
            //var department = new Department()
            //{
            //    Id = context.Message.Id,
            //    CreationDate = context.Message.Created,
            //    Description = context.Message.Description,
            //    Email = context.Message.Email,
            //    Name = context.Message.Name,
            //    PhoneNumber = context.Message.PhoneNumber,
            //};

            //await _userDbContext.AddAsync(department);

            //await _userDbContext.SaveChangesAsync(context.CancellationToken);

            _logger.Log(LogLevel.Information ,"Created Department");
        }
    }
}
