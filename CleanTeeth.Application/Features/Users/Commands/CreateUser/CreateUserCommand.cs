using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<long>, IRequireAction
    {
        public string RequiredActionName => "Users.Create";
        public required string Login { get; set; }
    }
}
