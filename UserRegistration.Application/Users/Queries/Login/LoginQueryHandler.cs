using MediatR;
using UserRegistration.Application.Abstractions.Repositories;
using UserRegistration.Application.Abstractions.Security;

namespace UserRegistration.Application.Users.Queries.Login
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, string>
    {
        private readonly IUsersRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginQueryHandler(IUsersRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<string> Handle(LoginQuery q, CancellationToken ct)
        {
            var email = q.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(q.Email, ct);
            if (user is null)
                throw new InvalidOperationException("User not found.");

            if (_passwordHasher.Verify(q.Password, user.PasswordHash, user.PasswordSalt))
                throw new InvalidOperationException("Incorrect password.");

            var name = $"{user.FirstName} {user.LastName}";

            return _jwtTokenGenerator.GenerateToken(user.Id, name, user.Email);
        }
    }
}
