using AccountService.Application.Queries.User;
using CustomHelper.Authentication.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetUserById(Ulid Id)
        {
            var user = await _mediator.Send(new GetUserQuery(Id));

            if (user == null)
            {
                return NotFound(Id);
            }

            return Ok(user);
        }
    }
}
