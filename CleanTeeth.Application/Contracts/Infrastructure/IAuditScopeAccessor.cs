namespace CleanTeeth.Application.Contracts.Infrastructure
{
    public interface IAuditScopeAccessor
    {
        AuditScopeContext Current { get; }
        IDisposable BeginScope(AuditScopeContext context);
    }

    public sealed class AuditScopeContext
    {
        public bool Enabled { get; init; }
        public bool IsLoggableAction { get; init; }
        public string? ActionName { get; init; }
        public string? UserId { get; init; }
        public string? Reason { get; init; }
    }
}
