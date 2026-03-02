using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.AppActions.Queries.GetActionDetail
{
    public class GetActionDetailQuery : IRequest<ActionDetailDTO>
    {
        public long Id { get; set; }
    }
}
