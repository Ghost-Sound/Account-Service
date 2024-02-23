using AccountService.Application.Commands.Departments;
using AccountService.Application.Commands.Users;
using AccountService.Application.Models.Departments;
using AccountService.Application.Models.Users;
using AccountService.Application.Queries.Departments;
using AccountService.Application.Queries.User;
using AccountService.Domain.Entity;
using CustomHelper.Authentication.Attributes;
using CustomHelper.Authentication.Enums;
using CustomHelper.Exception;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DepartmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(Ulid Id, CancellationToken cancellationToken)
        {
            try
            {
                var department = await _mediator.Send(new GetDepartmentQuery(Id), cancellationToken);

                if (department == null)
                {
                    return NotFound(Id);
                }

                return Ok(department);
            }
            catch
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartments([FromQuery] GetDepartmentsDTO model, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _mediator.Send(new GetDepartmentsQuery(model), cancellationToken);

                return Ok(users);
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentDTO model, CancellationToken cancellationToken)
        {
            try
            {
                var department = await _mediator.Send(new CreateDepartmentCommand(model), cancellationToken);
                var url = Url.Action(nameof(CreateDepartment), new { id = department.Id }) ?? $"/{department.Id}";

                return Created(url, department);
            }
            catch
            {
                throw;
            }
        }

        [JwtAuthorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartmentById(Ulid Id, CancellationToken cancellationToken)
        {
            try
            {
                var department = await _mediator.Send(new DeleteDepartmentCommand(Id), cancellationToken);

                return Ok(department);
            }
            catch
            {
                throw;
            }
        }

        [JwtAuthorize]
        [HttpPatch("{id}")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartmentById([FromBody] UpdateDepartmentDTO model, CancellationToken cancellationToken)
        {
            try
            {
                var department = await _mediator.Send(new UpdateDepartmentCommand(model), cancellationToken);

                if (department == null)
                {
                    throw new CustomException("Department is null");
                }

                return HttpContext.Request.Method switch
                {
                    "PATCH" => NoContent(),
                    "PUT" => Ok(department),
                    _ => BadRequest("Unsupported HTTP method"),
                };
            }
            catch
            {
                throw;
            }
        }
    }
}
