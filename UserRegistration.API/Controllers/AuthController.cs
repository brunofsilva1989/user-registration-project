using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserRegistration.Application.Dto;
using UserRegistration.Application.Users.Commands.Register;
using UserRegistration.Application.Users.Queries.Login;

namespace UserRegistration.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;
        public AuthController(ISender sender) => _sender = sender;

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Register(RegisterUserCommand cmd)
            => Ok(await _sender.Send(cmd));

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginQuery q)
            => Ok(new { token = await _sender.Send(q) });
    }
}
