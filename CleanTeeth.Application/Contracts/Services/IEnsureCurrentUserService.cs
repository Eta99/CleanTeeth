namespace CleanTeeth.Application.Contracts.Services
{
    public interface IEnsureCurrentUserService
    {
        /// <summary>
        /// Finds user by login or creates one if not found. Returns user info with roles and action names.
        /// </summary>
        Task<CurrentUserInfo?> EnsureAsync(string? login, CancellationToken cancellationToken = default);
    }
}
