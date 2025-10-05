using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRegistration.Application.Dto;

namespace UserRegistration.Application.Users.Queries.Login
{
    public sealed record LoginQuery(string Email, string Password) : IRequest<string>;
}
