using FluentValidation;
using TaskBoard.Application.Common.Exceptions;

namespace TaskBoard.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _requestDelegate = requestDelegate;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (ValidationException ee)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                var errors = ee.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(x => x.Key, x => x.Select(e => e.ErrorMessage).ToArray());

                await context.Response.WriteAsJsonAsync(new { errors });
            }
            catch(NotFoundException ee)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = ee.Message });
            }
            catch (ForbiddenException ee)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
            }
            catch (UnauthorizedAccessException ee)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = ee.Message });
            }
            catch (Exception ee)
            {
                _logger.LogError(ee, "Unhandled exception");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new {
                    message = ee.Message,
                    detail = ee.InnerException?.Message,
                    type = ee.GetType().Name
            });
            }
        }
    }
}
