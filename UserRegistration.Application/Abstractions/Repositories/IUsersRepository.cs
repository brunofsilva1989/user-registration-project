using UserRegistration.Domain.Entities;

namespace UserRegistration.Application.Abstractions.Repositories
{
    public interface IUsersRepository
    {
        Task<bool> EmailExistsAsync(string email, CancellationToken ct);
        Task AddAsync(UsersModel user, CancellationToken ct);
        Task<UsersModel?> GetByEmailAsync(string email, CancellationToken ct);
        Task<UsersModel?> GetByIdAsync(int id, CancellationToken ct);
        Task<List<UsersModel>> GetAllAsync(CancellationToken ct);
    }
}
