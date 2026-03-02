using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.Roles.Queries.GetRolesList
{
    public class GetRolesListQueryHandler : IRequestHandler<GetRolesListQuery, List<RoleListDTO>>
    {
        private readonly IRoleRepository _repository;

        public GetRolesListQueryHandler(IRoleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RoleListDTO>> Handle(GetRolesListQuery request)
        {
            var roles = await _repository.GetAll();
            return roles.Select(r => r.ToDTO()).ToList();
        }
    }
}
