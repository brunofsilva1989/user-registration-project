using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserRegistration.Application.Abstractions;
using UserRegistration.Application.Abstractions.Repositories;
using UserRegistration.Application.Abstractions.Security;
using UserRegistration.Application.Dto;
using UserRegistration.Domain.Entities;

namespace UserRegistration.Application.Users.Commands.Register
{
    public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
    {
        private readonly IUsersRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<RegisterUserCommandHandler> _log;

        public RegisterUserCommandHandler(IUsersRepository repo, IPasswordHasher hasher, IUnitOfWork uow, ILogger<RegisterUserCommandHandler> logger)
        { _userRepository = repo; _passwordHasher = hasher; _uow = uow; _log = logger; }

        public async Task<UserDto> Handle(RegisterUserCommand r, CancellationToken ct)
        {
            var email = r.email.Trim().ToLowerInvariant();

            if (await _userRepository.EmailExistsAsync(r.email, ct))
                throw new InvalidOperationException("E-mail already registered.");

            var (hash, salt) = _passwordHasher.Hash(r.password);
            var user = new UsersModel(r.login, r.firstName, r.lastName, r.email, hash, salt);
            
            _log?.LogInformation("State BEFORE Add: {state}", _uow is DbContext dc1 ? dc1.Entry(user).State : default);

            await _userRepository.AddAsync(user, ct);

            _log?.LogInformation("State AFTER Add: {state}", _uow is DbContext dc2 ? dc2.Entry(user).State : default);

            var committed = await _uow.SaveChangesAsync(ct); 

            if (committed == 0) throw new Exception("It was not possible to register the user.");


            return new UserDto(user.Id, user.Login, user.FirstName, user.LastName, user.Email, user.CreatedAt);
        }

    }
}
