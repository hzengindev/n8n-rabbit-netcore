using System;
namespace DeveloperAPI.Middlewares
{
	public static class MiddlewareSetupExtension
	{
        public static void MiddlewareSetup(this WebApplication app)
        {
            app.UseMiddleware<JWTMiddleware>();
            app.UseMiddleware<SecurityHeadersMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}

