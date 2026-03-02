using CleanTeeth.Domain.Entities;

namespace CleanTeeth.Application.Features.Roles.Queries.GetRoleDetail
{
    internal static class MapperExtensions
    {
        public static RoleDetailDTO ToDTO(this Role role)
        {
            var actions = role.Actions
                .Select(a => new ActionDTO { Id = a.Id, TypeId = a.TypeId, Name = a.Name, Title = a.Title })
                .ToList();
            return new RoleDetailDTO
            {
                Id = role.Id,
                Title = role.Title,
                Actions = actions
            };
        }
    }
}
