using MediatR;
using UserRegistration.Application.Abstractions.Repositories;
using UserRegistration.Application.Dto;

namespace UserRegistration.Application.Users.Queries.GetById
{
    public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
    {
        private readonly IUsersRepository _repo;

        public GetUserByIdQueryHandler(IUsersRepository repo) => _repo = repo;

        public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken ct)
        {
            var u = await _repo.GetByIdAsync(request.Id, ct);
            return u is null ? null : new UserDto(u.Id, u.Login, u.FirstName, u.LastName, u.Email, u.CreatedAt);
        }
    }
}
