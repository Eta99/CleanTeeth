using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.AppActions.Commands.UpdateAction
{
    public class UpdateActionCommand : IRequest
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public required string Title { get; set; }
    }
}
