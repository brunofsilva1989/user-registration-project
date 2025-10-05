using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserRegistration.Application.Abstractions.Security
{
    public interface IPasswordHasher
    {
        (byte[] hash, byte[] salt) Hash(string password);
        bool Verify(string password, byte[] hash, byte[] salt);
    }
}
