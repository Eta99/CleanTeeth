using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.AppActions.Commands.CreateAction
{
    public class CreateActionCommand : IRequest<long>, ILoggable
    {
        public long TypeId { get; set; }
        public required string Name { get; set; }
        public required string Title { get; set; }

        public string RequiredActionName => "CreateActions";
    }
}
