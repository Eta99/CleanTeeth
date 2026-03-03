using CleanTeeth.API.Jobs;
using CleanTeeth.API.Middlewares;
using CleanTeeth.Application;
using CleanTeeth.Infrastructure;
using CleanTeeth.Persistence;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // ?? ????????? ????????? ????????? ??????
    //options.FallbackPolicy = new AuthorizationPolicyBuilder()
    //    .RequireAssertion(_ => true)  // ??? options.DefaultPolicy ? AllowAnonymous
    //    .Build();

    // Require auth for endpoints without [Authorize].
    // This forces Negotiate handshake so HttpContext.User.Identity.IsAuthenticated becomes true.
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CleanTeeth.Application.Contracts.Services.ICurrentUserContext, CleanTeeth.API.Services.CurrentUserContext>();
builder.Services.AddPersistenceServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();




builder.Services.AddHostedService<AppointmentsReminderJob>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // ?????????? ????? ????: ??? ?????? ActionDTO (User/Role) ?? ???????????
    options.CustomSchemaIds(type => type.FullName ?? type.Name);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CleanTeeth API v1"));
}

app.UseCustomExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCurrentUser();
// Windows-?????????????? (Negotiate ???????? Kerberos ??? NTLM)


if (app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/" || context.Request.Path == "")
        {
            context.Response.Redirect("/swagger");
            return;
        }
        await next();
    });
}

app.MapControllers();

app.Run();
