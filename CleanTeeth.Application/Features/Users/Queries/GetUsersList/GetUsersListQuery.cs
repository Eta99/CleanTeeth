using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.Users.Queries.GetUsersList
{
    public class GetUsersListQuery : IRequest<List<UserListDTO>>, IRequireAction
    {
        public string RequiredActionName => "Users.View";
    }
}
