using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.Roles.Commands.DeleteRole
{
    public class DeleteRoleCommand : IRequest
    {
        public long Id { get; set; }
    }
}
