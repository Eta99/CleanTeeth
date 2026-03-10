using CleanTeeth.API.DTOs.Roles;
using CleanTeeth.Application.Features.Roles.Queries.GetRoleDetail;
using CleanTeeth.Application.Features.Roles.Queries.GetRolesList;
using CleanTeeth.Domain.Entities;

namespace CleanTeeth.API.ReferenceCrud.Mappers
{
    public class RoleReferenceCrudMapper : IReferenceCrudMapper<Role, RoleListDTO, RoleDetailDTO, CreateRoleDto, UpdateRoleDto>
    {
        public RoleListDTO ToListDto(Role entity)
        {
            return new RoleListDTO { Id = entity.Id, Title = entity.Title };
        }

        public RoleDetailDTO ToDetailDto(Role entity)
        {
            var actions = (entity.Actions ?? Enumerable.Empty<AppAction>())
                .Select(a => new ActionDTO { Id = a.Id, TypeId = a.TypeId, Name = a.Name, Title = a.Title })
                .ToList();
            return new RoleDetailDTO
            {
                Id = entity.Id,
                Title = entity.Title,
                Actions = actions
            };
        }

        public Role ToEntity(CreateRoleDto dto)
        {
            return new Role(dto.Title);
        }

        public void ApplyUpdate(Role entity, UpdateRoleDto dto)
        {
            entity.UpdateTitle(dto.Title);
        }

        public object GetId(Role entity)
        {
            return entity.Id;
        }
    }
}
