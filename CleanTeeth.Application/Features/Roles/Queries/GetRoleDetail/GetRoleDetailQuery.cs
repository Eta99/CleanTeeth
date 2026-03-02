using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.Roles.Queries.GetRoleDetail
{
    public class GetRoleDetailQuery : IRequest<RoleDetailDTO>
    {
        public long Id { get; set; }
    }
}
