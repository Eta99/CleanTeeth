using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.Roles.Commands.CreateRole
{
    public class CreateRoleCommand : IRequest<long>
    {
        public required string Title { get; set; }
    }
}
