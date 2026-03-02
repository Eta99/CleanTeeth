using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.Roles.Commands.UpdateRole
{
    public class UpdateRoleCommand : IRequest
    {
        public long Id { get; set; }
        public required string Title { get; set; }
    }
}
