using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest
    {
        public long Id { get; set; }
        public required string Login { get; set; }
    }
}
