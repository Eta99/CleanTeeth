using CleanTeeth.Application.Contracts.Services;

namespace CleanTeeth.API.Services
{
    public class CurrentUserContext : ICurrentUserContext
    {
        public const string CurrentUserKey = "CurrentUser";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
            }
        }

        public long? UserId => GetCurrentUserInfo()?.UserId;

        public string? Login => GetCurrentUserInfo()?.Login;

        public IReadOnlyList<long> RoleIds => GetCurrentUserInfo()?.RoleIds ?? Array.Empty<long>();

        public IReadOnlyList<string> ActionNames => GetCurrentUserInfo()?.ActionNames ?? Array.Empty<string>();

        public bool HasAction(string actionName)
        {
            var names = ActionNames;
            return names.Contains(actionName, StringComparer.OrdinalIgnoreCase);
        }

        private static CurrentUserInfo? GetCurrentUserInfo(HttpContext? context)
        {
            if (context?.Items.TryGetValue(CurrentUserKey, out var value) == true && value is CurrentUserInfo info)
                return info;
            return null;
        }

        private CurrentUserInfo? GetCurrentUserInfo() =>
            GetCurrentUserInfo(_httpContextAccessor.HttpContext);
    }
}
