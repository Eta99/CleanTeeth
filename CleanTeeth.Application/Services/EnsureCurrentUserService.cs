using CleanTeeth.Application.Contracts.Persistence;
using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Contracts.Services;
using CleanTeeth.Domain.Entities;

namespace CleanTeeth.Application.Services
{
    public class EnsureCurrentUserService : IEnsureCurrentUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EnsureCurrentUserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CurrentUserInfo?> EnsureAsync(string? login, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(login))
                return null;

            var user = await _userRepository.GetByLoginWithRolesAndActions(login);

            if (user is null)
            {
                user = new User(login);
                try
                {
                    await _userRepository.Add(user);
                    await _unitOfWork.Commit();
                }
                catch
                {
                    await _unitOfWork.Rollback();
                    throw;
                }
                // Reload with roles (empty for new user)
                user = await _userRepository.GetByLoginWithRolesAndActions(login);
                if (user is null)
                    return null;
            }

            var actionNames = user.Roles
                .SelectMany(r => r.Actions)
                .Select(a => a.Name)
                .Distinct()
                .ToList();

            return new CurrentUserInfo
            {
                UserId = user.Id,
                Login = user.Login,
                RoleIds = user.Roles.Select(r => r.Id).ToList(),
                ActionNames = actionNames
            };
        }
    }
}
