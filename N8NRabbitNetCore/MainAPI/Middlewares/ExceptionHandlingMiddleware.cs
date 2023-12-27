using System;
using System.Net;
using System.Text.Json;
using MainAPI.Core.Application.Exceptions;

namespace MainAPI.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next,
            IWebHostEnvironment env,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _env = env;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (RecordNotFoundException exception)
            {
                _logger.LogError(exception, exception.Message);

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    success = false,
                    message = exception.Message,
                }));
            }
            catch (AuthorizationException exception)
            {
                _logger.LogError(exception, exception.Message);

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = exception.StatusCode ?? 400;

                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    success = false,
                    message = exception.Message,
                }));
            }
            catch (AuthenticationException exception)
            {
                _logger.LogError(exception, exception.Message);

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = exception.StatusCode ?? 400;

                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    success = false,
                    message = exception.Message,
                }));
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(httpContext, exception);
            }
        }

        private Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            _logger.LogError(exception, exception.Message);

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (_env.EnvironmentName == "Development")
            {
                return httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    success = false,
                    message = exception.Message,
                    stackTrace = exception.StackTrace,
                }));
            }

            return httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                success = false,
                message = "Something went wrong.",
            }));
        }
    }
}

