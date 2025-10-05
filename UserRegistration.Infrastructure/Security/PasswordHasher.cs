using System.Security.Cryptography;
using System.Text;
using UserRegistration.Application.Abstractions.Security;


namespace UserRegistration.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public (byte[] hash, byte[] salt) Hash(string password)
        {
            using var hmac = new HMACSHA512();
            var salt = hmac.Key;
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return (hash, salt);
        }

        public bool Verify(string password, byte[] hash, byte[] salt)
        {
            using var hmac = new HMACSHA512(salt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(hash);
        }
    }
}
