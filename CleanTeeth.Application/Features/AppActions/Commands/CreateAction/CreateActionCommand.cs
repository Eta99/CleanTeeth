using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.AppActions.Commands.CreateAction
{
    public class CreateActionCommand : IRequest<long>
    {
        public required string Name { get; set; }
        public required string Title { get; set; }
    }
}
