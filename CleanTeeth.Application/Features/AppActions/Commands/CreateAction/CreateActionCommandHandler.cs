using CleanTeeth.Application.Contracts.Persistence;
using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Utilities;
using CleanTeeth.Domain.Entities;

namespace CleanTeeth.Application.Features.AppActions.Commands.CreateAction
{
    public class CreateActionCommandHandler : IRequestHandler<CreateActionCommand, long>
    {
        private readonly IAppActionRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateActionCommandHandler(IAppActionRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreateActionCommand command)
        {
            var action = new AppAction(command.Name, command.Title);
            try
            {
                var result = await _repository.Add(action);
                await _unitOfWork.Commit();
                return result.Id;
            }
            catch (Exception)
            {
                await _unitOfWork.Rollback();
                throw;
            }
        }
    }
}
