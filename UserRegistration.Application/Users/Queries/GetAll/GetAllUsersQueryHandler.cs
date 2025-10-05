using MediatR;
using UserRegistration.Application.Abstractions.Repositories;
using UserRegistration.Application.Dto;

namespace UserRegistration.Application.Users.Queries.GetAll
{
    public sealed class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
    {
        private readonly IUsersRepository _repo;

        public GetAllUsersQueryHandler(IUsersRepository repo) => _repo = repo;

        public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken ct)
        {
            var users = await _repo.GetAllAsync(ct);
            return users.Select(u => new UserDto(u.Id, u.Login, u.FirstName, u.LastName, u.Email, u.CreatedAt));
        }
    }
}
