namespace CleanTeeth.Application.Contracts.Services
{
    public class CurrentUserInfo
    {
        public long UserId { get; init; }
        public string Login { get; init; } = null!;
        public IReadOnlyList<long> RoleIds { get; init; } = Array.Empty<long>();
        public IReadOnlyList<string> ActionNames { get; init; } = Array.Empty<string>();
    }
}
