using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserRegistration.Application.Dto;
using UserRegistration.Application.Users.Queries.GetAll;
using UserRegistration.Application.Users.Queries.GetById;

namespace UserRegistration.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ISender sender, ILogger<UsersController> logger)
        {
            _sender = sender;
            _logger = logger;
        }

        // GET /api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            _logger.LogInformation("Fetching all users");
            var result = await _sender.Send(new GetAllUsersQuery());
            _logger.LogInformation("Fetched {UserCount} users", result.Count());

            return Ok(result);
        }

        //GET /api/users/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            _logger.LogInformation("Searching for user with ID {UserId}", id);
            var user = await _sender.Send(new GetUserByIdQuery(id));
            _logger.LogInformation("Search completed for user with ID {UserId}", id);

            if (user is null) return NotFound();
            _logger.LogInformation("Retrieved user with ID {UserId}", id);

            return Ok(user);
        }
    }
}
