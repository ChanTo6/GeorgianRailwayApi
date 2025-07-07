using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using GeorgianRailwayApi.Exceptions;

namespace GeorgianRailwayApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainException dex)
            {
                _logger.LogWarning(dex, "Domain exception occurred");
                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = dex is NotFoundException ? (int)HttpStatusCode.NotFound : (int)HttpStatusCode.BadRequest;
                var problem = new
                {
                    type = dex is NotFoundException ? "https://httpstatuses.com/404" : "https://httpstatuses.com/400",
                    title = dex.GetType().Name,
                    status = context.Response.StatusCode,
                    detail = dex.Message,
                    errorCode = dex.ErrorCode
                };
                var json = JsonSerializer.Serialize(problem);
                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var problem = new
                {
                    type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    title = "An unexpected error occurred.",
                    status = context.Response.StatusCode,
                    detail = "An internal server error occurred. Please contact support if the problem persists."
                };
                var json = JsonSerializer.Serialize(problem);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
