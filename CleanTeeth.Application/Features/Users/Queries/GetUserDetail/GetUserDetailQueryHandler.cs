using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Contracts.Services;
using CleanTeeth.Application.Exceptions;
using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.Users.Queries.GetUserDetail
{
    public class GetUserDetailQueryHandler : IRequestHandler<GetUserDetailQuery, UserDetailDTO>
    {
        private readonly IUserRepository _repository;
        private readonly ICurrentUserContext _currentUserContext;

        public GetUserDetailQueryHandler(IUserRepository repository, ICurrentUserContext currentUserContext)
        {
            _repository = repository;
            _currentUserContext = currentUserContext;
        }

        public async Task<UserDetailDTO> Handle(GetUserDetailQuery request)
        {
            var _ = _currentUserContext.UserId;

            var user = await _repository.GetByIdWithRolesAndActions(request.Id);
            if (user is null)
                throw new NotFoundException();
            return user.ToDTO();
        }
    }
}

