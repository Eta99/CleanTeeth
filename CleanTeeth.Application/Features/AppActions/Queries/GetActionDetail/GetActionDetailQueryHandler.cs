using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Exceptions;
using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.AppActions.Queries.GetActionDetail
{
    public class GetActionDetailQueryHandler : IRequestHandler<GetActionDetailQuery, ActionDetailDTO>
    {
        private readonly IAppActionRepository _repository;

        public GetActionDetailQueryHandler(IAppActionRepository repository)
        {
            _repository = repository;
        }

        public async Task<ActionDetailDTO> Handle(GetActionDetailQuery request)
        {
            var action = await _repository.GetById(request.Id);
            if (action is null)
                throw new NotFoundException();
            return action.ToDTO();
        }
    }
}
