using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Exceptions;
using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.Users.Queries.GetUserDetail
{
    public class GetUserDetailQueryHandler : IRequestHandler<GetUserDetailQuery, UserDetailDTO>
    {
        private readonly IUserRepository _repository;

        public GetUserDetailQueryHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserDetailDTO> Handle(GetUserDetailQuery request)
        {
            var user = await _repository.GetByIdWithRolesAndActions(request.Id);
            if (user is null)
                throw new NotFoundException();
            return user.ToDTO();
        }
    }
}
