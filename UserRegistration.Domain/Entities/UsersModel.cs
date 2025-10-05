using System.ComponentModel.DataAnnotations;

namespace UserRegistration.Domain.Entities
{
    public class UsersModel
    {        
        public UsersModel(string login, string firstName, string lastName, string email, byte[] passwordHash, byte[] passwordSalt)
        {
            Login = login;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            CreatedAt = DateTime.UtcNow;
        }

        [Key]
        public int Id { get; private set; }
        [Required]
        public string Login { get; private set; } = default!;
        [Required]
        public string FirstName { get; private set; } = default!;
        [Required]
        public string LastName { get; private set; } = default!;
        [Required]
        public string Email { get; private set; } = default!;
        public byte[] PasswordHash { get; private set; } = default!;
        public byte[] PasswordSalt { get; private set; } = default!;
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
    }
}
