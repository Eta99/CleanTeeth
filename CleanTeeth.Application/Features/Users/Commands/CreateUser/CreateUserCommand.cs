using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<long>
    {
        public required string Login { get; set; }
    }
}
