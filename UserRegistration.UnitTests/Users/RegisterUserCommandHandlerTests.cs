using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using UserRegistration.Application.Abstractions;
using UserRegistration.Application.Abstractions.Repositories;
using UserRegistration.Application.Abstractions.Security;
using UserRegistration.Application.Dto;
using UserRegistration.Application.Users.Commands.Register;
using UserRegistration.Domain.Entities;
using Xunit;

namespace UserRegistration.UnitTests.Users
{
    public class RegisterUserCommandHandlerTests
    {
        private readonly Mock<IUsersRepository> _repo = new();
        private readonly Mock<IPasswordHasher> _hasher = new();
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly Mock<ILogger<RegisterUserCommandHandler>> _logger = new();

        [Fact]
        public async Task Handle_ShouldCreateUser_WhenEmailDoesNotExist()
        {
            // Arrange
            var cmd = new RegisterUserCommand(
                login: "jsilva",
                firstName: "João",
                lastName: "Silva",
                email: "joao@exemplo.com",
                password: "P@ssw0rd"
            );

            _repo.Setup(r => r.EmailExistsAsync(cmd.email, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

            _hasher.Setup(h => h.Hash(cmd.password))
               .Returns((Encoding.UTF8.GetBytes("hash"), Encoding.UTF8.GetBytes("salt")));

            _repo.Setup(r => r.AddAsync(It.IsAny<UsersModel>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

            _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var handler = new RegisterUserCommandHandler(_repo.Object, _hasher.Object, _uow.Object, _logger.Object);

            // Act
            UserDto result = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(cmd.email);
            result.Login.Should().Be(cmd.login);

            _repo.Verify(r => r.AddAsync(
                It.Is<UsersModel>(u => u.Email == cmd.email && u.Login == cmd.login),
                It.IsAny<CancellationToken>()), Times.Once);

            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenEmailAlreadyExists()
        {
            // Arrange
            var cmd = new RegisterUserCommand(
                login: "jsilva",
                firstName: "João",
                lastName: "Silva",
                email: "joao@exemplo.com",
                password: "P@ssw0rd"
            );

            _repo.Setup(r => r.EmailExistsAsync(cmd.email, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

            var handler = new RegisterUserCommandHandler(_repo.Object, _hasher.Object, _uow.Object, _logger.Object);

            // Act
            var act = async () => await handler.Handle(cmd, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("*E-mail*"); 
            _repo.Verify(r => r.AddAsync(It.IsAny<UsersModel>(), It.IsAny<CancellationToken>()), Times.Never);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
