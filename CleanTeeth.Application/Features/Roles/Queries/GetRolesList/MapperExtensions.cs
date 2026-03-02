using CleanTeeth.Domain.Entities;

namespace CleanTeeth.Application.Features.Roles.Queries.GetRolesList
{
    internal static class MapperExtensions
    {
        public static RoleListDTO ToDTO(this Role role)
        {
            return new RoleListDTO { Id = role.Id, Title = role.Title };
        }
    }
}
