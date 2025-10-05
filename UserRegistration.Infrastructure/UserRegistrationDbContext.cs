using Microsoft.EntityFrameworkCore;
using UserRegistration.Application.Abstractions;
using UserRegistration.Domain.Entities;

namespace UserRegistration.Infrastructure
{
    public class UserRegistrationDbContext : DbContext, IUnitOfWork
    {
        public UserRegistrationDbContext(DbContextOptions<UserRegistrationDbContext> options) : base(options)
        {
        }

        public DbSet<UsersModel> Users => Set<UsersModel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UsersModel>().ToTable("Users");
            modelBuilder.Entity<UsersModel>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}
