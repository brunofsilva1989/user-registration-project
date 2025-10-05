using MediatR;
using UserRegistration.Application.Dto;

namespace UserRegistration.Application.Users.Queries.GetAll
{
    public sealed record GetAllUsersQuery : IRequest<IEnumerable<UserDto>>;
}
