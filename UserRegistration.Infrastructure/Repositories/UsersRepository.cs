using Microsoft.EntityFrameworkCore;
using UserRegistration.Application.Abstractions.Repositories;
using UserRegistration.Domain.Entities;

namespace UserRegistration.Infrastructure.Repositories
{
    public sealed class UsersRepository : IUsersRepository
    {
        private readonly UserRegistrationDbContext _context;
        public UsersRepository(UserRegistrationDbContext context) => _context = context;

        public Task<bool> EmailExistsAsync(string email, CancellationToken ct)
            => _context.Users.AnyAsync(u => u.Email == email, ct);

        public Task<UsersModel?> GetByEmailAsync(string email, CancellationToken ct)
            => _context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

        public Task<UsersModel?> GetByIdAsync(int id, CancellationToken ct)
            => _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

        public Task AddAsync(UsersModel user, CancellationToken ct)
            => _context.Users.AddAsync(user, ct).AsTask();

        public Task<List<UsersModel>> GetAllAsync(CancellationToken ct)
            => _context.Users.AsNoTracking().ToListAsync(ct);        
    }
}
