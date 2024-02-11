using AccountService.Application.Commands.Users;
using AccountService.Application.Queries.User;
using CustomHelper.Authentication.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CustomHelper.Exception;
using CustomHelper.Authentication.Attributes;
using AccountService.Application.Models.Users;

namespace AccountService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ISignInKeys _signInKeys;
        private readonly IMediator _mediator;

        public UserController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ISignInKeys signInKeys,
            IMediator mediator)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _signInKeys = signInKeys;
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Ulid Id, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _mediator.Send(new GetUserQuery(Id), cancellationToken);

                if (user == null)
                {
                    return NotFound(Id);
                }

                return Ok(user);
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetUsers([FromBody] UsersGetDTO model, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _mediator.Send(new GetUsersQuery(model), cancellationToken);

                return Ok(users);
            }
            catch
            {
                throw;
            }
        }

        [JwtAuthorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserById(Ulid Id, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _mediator.Send(new DeleteUserCommand(Id), cancellationToken);

                return Ok(user);
            }
            catch
            {
                throw;
            }
        }

        [JwtAuthorize]
        [HttpPatch("{id}")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserById([FromBody] UserUpdateDTO model, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _mediator.Send(new UpdateUserCommand(model), cancellationToken);

                if (user == null)
                {
                    throw new CustomException("User is null");
                }

                return HttpContext.Request.Method switch
                {
                    "PATCH" => NoContent(),
                    "PUT" => Ok(user),
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
