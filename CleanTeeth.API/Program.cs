using CleanTeeth.API.Jobs;
using CleanTeeth.API.Middlewares;
using CleanTeeth.Application;
using CleanTeeth.Infrastructure;
using CleanTeeth.Persistence;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // ѕо умолчанию разрешаем анонимный доступ
    //options.FallbackPolicy = new AuthorizationPolicyBuilder()
    //    .RequireAssertion(_ => true)  // или options.DefaultPolicy с AllowAnonymous
    //    .Build();

    options.FallbackPolicy = null;
});

builder.Services.AddPersistenceServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();




builder.Services.AddHostedService<AppointmentsReminderJob>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCustomExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();   // добавить
app.UseAuthorization();
// Windows-аутентификаци€ (Negotiate выбирает Kerberos или NTLM)




app.MapControllers();

app.Run();
