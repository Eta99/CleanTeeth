using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.AppActions.Commands.DeleteAction
{
    public class DeleteActionCommand : IRequest
    {
        public long Id { get; set; }
    }
}
