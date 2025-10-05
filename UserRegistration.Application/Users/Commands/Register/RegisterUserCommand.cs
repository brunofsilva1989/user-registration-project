using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRegistration.Application.Dto;

namespace UserRegistration.Application.Users.Commands.Register
{
    public sealed record RegisterUserCommand(string login, string firstName, string lastName, string email, string password) : IRequest<UserDto>;
}
