using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Exceptions;
using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.Roles.Queries.GetRoleDetail
{
    public class GetRoleDetailQueryHandler : IRequestHandler<GetRoleDetailQuery, RoleDetailDTO>
    {
        private readonly IRoleRepository _repository;

        public GetRoleDetailQueryHandler(IRoleRepository repository)
        {
            _repository = repository;
        }

        public async Task<RoleDetailDTO> Handle(GetRoleDetailQuery request)
        {
            var role = await _repository.GetByIdWithActions(request.Id);
            if (role is null)
                throw new NotFoundException();
            return role.ToDTO();
        }
    }
}
