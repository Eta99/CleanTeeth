using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.Users.Commands.DeleteUser
{
    public class DeleteUserCommand : IRequest
    {
        public long Id { get; set; }
    }
}
