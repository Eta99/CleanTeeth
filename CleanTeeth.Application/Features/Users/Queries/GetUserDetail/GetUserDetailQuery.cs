using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.Users.Queries.GetUserDetail
{
    public class GetUserDetailQuery : IRequest<UserDetailDTO>
    {
        public long Id { get; set; }
    }
}
