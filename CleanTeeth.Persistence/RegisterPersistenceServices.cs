using CleanTeeth.Application.Contracts.Persistence;
using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Domain.Entities;
using CleanTeeth.Persistence.Repositories;
using CleanTeeth.Persistence.UnitsOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanTeeth.Persistence
{
    public static class RegisterPersistenceServices
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
        {
            services.AddDbContext<CleanTeethDbContext>(options =>
                options.UseSqlServer("name=CleanTeethConnectionString"));

            services.AddScoped<IDentalOfficeRepository, DentalOfficeRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IDentistRepository, DentistRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IAppActionRepository, AppActionRepository>();
            services.AddScoped<ILogRepository, LogRepository>();

            // Регистрация IRepository<T> / IRepositoryLongKey<T> по типу сущности для универсального CRUD
            services.AddScoped<IRepository<DentalOffice>>(sp => sp.GetRequiredService<IDentalOfficeRepository>());
            services.AddScoped<IRepository<Patient>>(sp => sp.GetRequiredService<IPatientRepository>());
            services.AddScoped<IRepository<Dentist>>(sp => sp.GetRequiredService<IDentistRepository>());
            services.AddScoped<IRepository<Appointment>>(sp => sp.GetRequiredService<IAppointmentRepository>());
            services.AddScoped<IRepositoryLongKey<User>>(sp => sp.GetRequiredService<IUserRepository>());
            services.AddScoped<IRepositoryLongKey<Role>>(sp => sp.GetRequiredService<IRoleRepository>());
            services.AddScoped<IRepositoryLongKey<AppAction>>(sp => sp.GetRequiredService<IAppActionRepository>());
            services.AddScoped<IRepositoryLongKey<Log>>(sp => sp.GetRequiredService<ILogRepository>());

            services.AddScoped<IUnitOfWork, UnitOfWorkEFCore>();

            return services;
        }
    }
}
