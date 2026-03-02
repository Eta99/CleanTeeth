namespace CleanTeeth.Application.Utilities
{
    /// <summary>
    /// Marks a request (command/query) as requiring the current user to have the specified action.
    /// The mediator will check ICurrentUserContext.HasAction(RequiredActionName) before invoking the handler.
    /// </summary>
    public interface IRequireAction
    {
        string RequiredActionName { get; }
    }
}
