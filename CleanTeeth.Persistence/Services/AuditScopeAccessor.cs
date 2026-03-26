using CleanTeeth.Application.Contracts.Infrastructure;
using System.Threading;

namespace CleanTeeth.Persistence.Services
{
    internal sealed class AuditScopeAccessor : IAuditScopeAccessor
    {
        private static readonly AsyncLocal<AuditScopeContext?> Context = new();
        private static readonly AuditScopeContext DisabledContext = new()
        {
            Enabled = false,
            IsLoggableAction = false
        };

        public AuditScopeContext Current => Context.Value ?? DisabledContext;

        public IDisposable BeginScope(AuditScopeContext context)
        {
            var previous = Context.Value;
            Context.Value = context;
            return new ScopeReset(() => Context.Value = previous);
        }

        private sealed class ScopeReset : IDisposable
        {
            private readonly Action reset;
            private bool disposed;

            public ScopeReset(Action reset)
            {
                this.reset = reset;
            }

            public void Dispose()
            {
                if (disposed)
                    return;

                disposed = true;
                reset();
            }
        }
    }
}
