using CleanTeeth.Domain.Entities;

namespace CleanTeeth.Application.Features.Users.Queries.GetUserDetail
{
    internal static class MapperExtensions
    {
        public static UserDetailDTO ToDTO(this User user)
        {
            var roles = user.Roles.Select(r => new RoleDTO { Id = r.Id, Title = r.Title }).ToList();
            var actions = user.Roles
                .SelectMany(r => r.Actions)
                .GroupBy(a => a.Id)
                .Select(g => g.First())
                .Select(a => new ActionDTO { Id = a.Id, TypeId = a.TypeId, Name = a.Name, Title = a.Title })
                .ToList();

            return new UserDetailDTO
            {
                Id = user.Id,
                Login = user.Login,
                Roles = roles,
                Actions = actions
            };
        }
    }
}
