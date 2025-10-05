namespace UserRegistration.Application.Dto
{
    public sealed record UserDto(int Id, string Login, string FirstName, string LastName, string Email, DateTime CreateAt);

}
