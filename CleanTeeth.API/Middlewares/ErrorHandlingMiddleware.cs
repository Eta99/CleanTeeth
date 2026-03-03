using CleanTeeth.Application.Exceptions;
using CleanTeeth.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace CleanTeeth.API.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;

        public ErrorHandlingMiddleware(RequestDelegate next, IHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private Task HandleException(HttpContext context, Exception exception)
        {
            if (exception is System.Reflection.TargetInvocationException { InnerException: { } inner })
                exception = inner;

            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var result = string.Empty;

            switch (exception)
            {
                case NotFoundException:
                    httpStatusCode = HttpStatusCode.NotFound;
                    break;
                case ForbiddenException:
                    httpStatusCode = HttpStatusCode.Forbidden;
                    break;
                case BusinessRuleException businessRuleException:
                    httpStatusCode = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(businessRuleException.Message);
                    break;
                case CustomValidationException customValidationException:
                    httpStatusCode = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(customValidationException.ValidationErrors);
                    break;
                case MediatorException mediatorEx:
                    httpStatusCode = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new { error = mediatorEx.Message });
                    break;
                case InvalidOperationException invalidOp when invalidOp.Message.Contains("Repository", StringComparison.OrdinalIgnoreCase):
                    httpStatusCode = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new { error = invalidOp.Message });
                    break;
            }

            if ((int)httpStatusCode == 500)
            {
                if (_env.IsDevelopment() && string.IsNullOrEmpty(result))
                {
                    result = JsonSerializer.Serialize(new
                    {
                        error = exception.Message,
                        type = exception.GetType().Name,
                        stackTrace = exception.StackTrace
                    });
                }
                else if (string.IsNullOrEmpty(result))
                {
                    result = JsonSerializer.Serialize(new { error = "An error occurred." });
                }
            }

            context.Response.StatusCode = (int)httpStatusCode;
            return context.Response.WriteAsync(result);
        }
    }

    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }

}
