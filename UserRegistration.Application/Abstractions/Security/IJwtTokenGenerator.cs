using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserRegistration.Application.Abstractions.Security
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(int userId, string name, string email);
    }
}
