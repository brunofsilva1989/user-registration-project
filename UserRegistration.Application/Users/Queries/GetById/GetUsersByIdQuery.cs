using MediatR;
using UserRegistration.Application.Dto;

namespace UserRegistration.Application.Users.Queries.GetById
{
    public sealed record GetUserByIdQuery(int Id) : IRequest<UserDto?>;
}
