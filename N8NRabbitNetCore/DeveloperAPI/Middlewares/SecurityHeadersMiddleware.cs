using System;
namespace DeveloperAPI.Middlewares
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-XSS-Protection
            httpContext.Response.Headers.Add("X-Xss-Protection", "1; mode=block");

            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
            httpContext.Response.Headers.Add("X-Content-Type-Options", "nosniff");

            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
            httpContext.Response.Headers.Add("X-Frame-Options", "DENY");

            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy
            httpContext.Response.Headers.Add("Referrer-Policy", "no-referrer"); // strict-origin-when-cross-origin

            await _next(httpContext);
        }
    }
}

