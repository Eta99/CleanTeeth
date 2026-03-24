using CleanTeeth.Application.Contracts.Services;
using CleanTeeth.Application.Features.DentalOffices.Commands.CreateDentalOffice;
using CleanTeeth.Application.Features.DentalOffices.Commands.DeleteDentalOffice;
using CleanTeeth.Application.Features.DentalOffices.Commands.UpdateDentalOffice;
using CleanTeeth.Application.Features.DentalOffices.Queries.GetDentalOfficeDetail;
using CleanTeeth.Application.Features.DentalOffices.Queries.GetDentalOfficesList;
using CleanTeeth.Application.Features.UniversalCrud.Commands.Create;
using CleanTeeth.Application.Features.UniversalCrud.Commands.Delete;
using CleanTeeth.Application.Features.UniversalCrud.Commands.Update;
using CleanTeeth.Application.Features.UniversalCrud.Queries.GetAll;
using CleanTeeth.Application.Features.UniversalCrud.Queries.GetById;
using CleanTeeth.Application.Services;
using CleanTeeth.Application.Utilities;
using CleanTeeth.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanTeeth.Application
{
    public static class RegisterApplicationServices
    {
        private static readonly Type[] UniversalCrudEntityTypes =
        {
            typeof(DentalOffice), typeof(Patient), typeof(Dentist), typeof(Appointment),
            typeof(User), typeof(Role), typeof(AppAction)
        };

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IMediator, SimpleMediator>();
            services.AddScoped<ChangeLogSession>();
            services.AddScoped<MediatorChangeLogCoordinator>();
            services.AddScoped<IEnsureCurrentUserService, EnsureCurrentUserService>();

            services.Scan(scan => scan.FromAssembliesOf(typeof(RegisterApplicationServices))
                .AddClasses(c => c.AssignableTo(typeof(IRequestHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
                .AddClasses(c => c.AssignableTo(typeof(IRequestHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            RegisterUniversalCrudGenericHandlers(services);


            //services.AddScoped<IRequestHandler<CreateDentalOfficeCommand, Guid>, CreateDentalOfficeCommandHandler>();
            //services.AddScoped<IRequestHandler<GetDentalOfficeDetailQuery, DentalOfficeDetailDTO>,
            //                    GetDentalOfficeDetailQueryHandler>();

            //services.AddScoped<IRequestHandler<GetDentalOfficesListQuery, List<DentalOfficesListDTO>>,
            //    GetDentalOfficesListQueryHandler>();

            //services.AddScoped<IRequestHandler<UpdateDentalOfficeCommand>,
            //    UpdateDentalOfficeCommandHandler>();

            //services.AddScoped<IRequestHandler<DeleteDentalOfficeCommand>, 
            //    DeleteDentalOfficeCommandHandler>();

            return services;
        }

        private static void RegisterUniversalCrudGenericHandlers(IServiceCollection services)
        {
            foreach (var entityType in UniversalCrudEntityTypes)
            {
                var getByIdQueryType = typeof(GetByIdQuery<>).MakeGenericType(entityType);
                var getByIdHandlerType = typeof(GetByIdQueryHandler<>).MakeGenericType(entityType);
                services.AddScoped(typeof(IRequestHandler<,>).MakeGenericType(getByIdQueryType, entityType), getByIdHandlerType);

                var getAllQueryType = typeof(GetAllQuery<>).MakeGenericType(entityType);
                var getAllResultType = typeof(IEnumerable<>).MakeGenericType(entityType);
                var getAllHandlerType = typeof(GetAllQueryHandler<>).MakeGenericType(entityType);
                services.AddScoped(typeof(IRequestHandler<,>).MakeGenericType(getAllQueryType, getAllResultType), getAllHandlerType);

                var createCommandType = typeof(CreateCommand<>).MakeGenericType(entityType);
                var createHandlerType = typeof(CreateCommandHandler<>).MakeGenericType(entityType);
                services.AddScoped(typeof(IRequestHandler<,>).MakeGenericType(createCommandType, entityType), createHandlerType);

                var updateCommandType = typeof(UpdateCommand<>).MakeGenericType(entityType);
                var updateHandlerType = typeof(UpdateCommandHandler<>).MakeGenericType(entityType);
                services.AddScoped(typeof(IRequestHandler<>).MakeGenericType(updateCommandType), updateHandlerType);

                var deleteCommandType = typeof(DeleteCommand<>).MakeGenericType(entityType);
                var deleteHandlerType = typeof(DeleteCommandHandler<>).MakeGenericType(entityType);
                services.AddScoped(typeof(IRequestHandler<>).MakeGenericType(deleteCommandType), deleteHandlerType);
            }
        }
    }
}
