using System;
namespace MainAPI.Middlewares
{
	public static class MiddlewareSetupExtension
	{
        public static void MiddlewareSetup(this WebApplication app)
        {
            app.UseMiddleware<SecurityHeadersMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}

