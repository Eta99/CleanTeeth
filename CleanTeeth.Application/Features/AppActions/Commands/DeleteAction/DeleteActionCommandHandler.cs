using CleanTeeth.Application.Contracts.Persistence;
using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Exceptions;
using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.AppActions.Commands.DeleteAction
{
    public class DeleteActionCommandHandler : IRequestHandler<DeleteActionCommand>
    {
        private readonly IAppActionRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteActionCommandHandler(IAppActionRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteActionCommand request)
        {
            var action = await _repository.GetById(request.Id);
            if (action is null)
                throw new NotFoundException();

            try
            {
                await _repository.Delete(action);
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
