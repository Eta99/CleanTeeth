using CleanTeeth.Application.Contracts.Services;
using CleanTeeth.Application.Exceptions;
using CleanTeeth.Application.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;

namespace CleanTeeth.Application.Utilities
{
    public class SimpleMediator : IMediator
    {
        private readonly IServiceProvider serviceProvider;

        public SimpleMediator(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
        {
            await ApplyValidations(request).ConfigureAwait(false);
            await CheckRequiredAction(request).ConfigureAwait(false);

            MediatorChangeLogCoordinator? logCoordinator = null;
            if (request is ILoggable)
                logCoordinator = serviceProvider.GetRequiredService<MediatorChangeLogCoordinator>();

            if (logCoordinator != null)
                await logCoordinator.PrepareAsync(request).ConfigureAwait(false);

            try
            {
                var handlerType = typeof(IRequestHandler<,>)
                    .MakeGenericType(request.GetType(), typeof(TResponse));

                var handler = serviceProvider.GetService(handlerType);

                if (handler is null)
                    throw new MediatorException($"Handler was not found for {request.GetType().Name}");

                var method = handlerType.GetMethod("Handle")!;
                var result = await ((Task<TResponse>)method.Invoke(handler, new object[] { request })!).ConfigureAwait(false);

                if (logCoordinator != null)
                    await logCoordinator.PersistAfterSuccessAsync(request, result).ConfigureAwait(false);

                return result;
            }
            catch
            {
                logCoordinator?.DiscardPrepared();
                throw;
            }
        }

        public async Task Send(IRequest request)
        {
            await ApplyValidations(request).ConfigureAwait(false);
            await CheckRequiredAction(request).ConfigureAwait(false);

            MediatorChangeLogCoordinator? logCoordinator = null;
            if (request is ILoggable)
                logCoordinator = serviceProvider.GetRequiredService<MediatorChangeLogCoordinator>();

            if (logCoordinator != null)
                await logCoordinator.PrepareAsync(request).ConfigureAwait(false);

            try
            {
                var handlerType = typeof(IRequestHandler<>).MakeGenericType(request.GetType());

                var handler = serviceProvider.GetService(handlerType);

                if (handler is null)
                    throw new MediatorException($"Handler was not found for {request.GetType().Name}");

                var method = handlerType.GetMethod("Handle")!;
                await ((Task)method.Invoke(handler, new object[] { request })!).ConfigureAwait(false);

                if (logCoordinator != null)
                    await logCoordinator.PersistAfterSuccessAsync(request, response: null).ConfigureAwait(false);
            }
            catch
            {
                logCoordinator?.DiscardPrepared();
                throw;
            }
        }

        private Task CheckRequiredAction(object request)
        {
            var currentUser = serviceProvider.GetService(typeof(ICurrentUserContext)) as ICurrentUserContext;
            if (currentUser is null || !currentUser.IsAuthenticated)
                throw new ForbiddenException();

            if (request is IRequireAction requireAction && !currentUser.HasAction(requireAction.RequiredActionName))
                throw new ForbiddenException();

            return Task.CompletedTask;
        }

        private async Task ApplyValidations(object request)
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(request.GetType());

            var validator = serviceProvider.GetService(validatorType);

            if (validator is not null)
            {
                var validateMethod = validatorType.GetMethod("ValidateAsync");
                var taskToValidate = (Task)validateMethod!.Invoke(validator, new object[] { request, CancellationToken.None })!;

                await taskToValidate.ConfigureAwait(false);

                var result = taskToValidate.GetType().GetProperty("Result");
                var validationResult = (ValidationResult)result!.GetValue(taskToValidate)!;

                if (!validationResult.IsValid)
                    throw new CustomValidationException(validationResult);
            }
        }
    }
}
