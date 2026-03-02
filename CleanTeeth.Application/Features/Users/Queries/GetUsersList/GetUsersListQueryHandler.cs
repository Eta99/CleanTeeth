using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.Users.Queries.GetUsersList
{
    public class GetUsersListQueryHandler : IRequestHandler<GetUsersListQuery, List<UserListDTO>>
    {
        private readonly IUserRepository _repository;

        public GetUsersListQueryHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<UserListDTO>> Handle(GetUsersListQuery request)
        {
            var users = await _repository.GetAll();
            return users.Select(u => u.ToDTO()).ToList();
        }
    }
}
