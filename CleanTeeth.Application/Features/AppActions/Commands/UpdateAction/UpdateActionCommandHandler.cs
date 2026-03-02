using CleanTeeth.Application.Contracts.Persistence;
using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Exceptions;
using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.AppActions.Commands.UpdateAction
{
    public class UpdateActionCommandHandler : IRequestHandler<UpdateActionCommand>
    {
        private readonly IAppActionRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateActionCommandHandler(IAppActionRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateActionCommand request)
        {
            var action = await _repository.GetById(request.Id);
            if (action is null)
                throw new NotFoundException();

            action.Update(request.Name, request.Title);

            try
            {
                await _repository.Update(action);
                await _unitOfWork.Commit();
            }
            catch (Exception)
            {
                await _unitOfWork.Rollback();
                throw;
            }
        }
    }
}
