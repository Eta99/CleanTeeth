using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.AppActions.Queries.GetActionsList
{
    public class GetActionsListQueryHandler : IRequestHandler<GetActionsListQuery, List<ActionListDTO>>
    {
        private readonly IAppActionRepository _repository;

        public GetActionsListQueryHandler(IAppActionRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ActionListDTO>> Handle(GetActionsListQuery request)
        {
            var actions = await _repository.GetAll();
            return actions.Select(a => a.ToDTO()).ToList();
        }
    }
}
