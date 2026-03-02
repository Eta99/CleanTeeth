namespace CleanTeeth.Application.Contracts.Services
{
    public interface ICurrentUserContext
    {
        bool IsAuthenticated { get; }
        long? UserId { get; }
        string? Login { get; }
        IReadOnlyList<long> RoleIds { get; }
        IReadOnlyList<string> ActionNames { get; }
        bool HasAction(string actionName);
    }
}
