using CleanTeeth.Domain.Entities;

namespace CleanTeeth.Application.Features.Users.Queries.GetUsersList
{
    internal static class MapperExtensions
    {
        public static UserListDTO ToDTO(this User user)
        {
            return new UserListDTO
            {
                Id = user.Id,
                Login = user.Login
            };
        }
    }
}
