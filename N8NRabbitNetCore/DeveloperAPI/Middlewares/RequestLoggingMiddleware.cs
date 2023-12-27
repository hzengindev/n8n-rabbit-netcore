using System;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DeveloperAPI.Core.Application.DTOs.SystemUser;

namespace DeveloperAPI.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Items.ContainsKey("CurrentUser"))
            {
                var user = httpContext.Items["CurrentUser"] as SystemUserDTO;
                LogContext.PushProperty("username", user.Username);
                LogContext.PushProperty("fullname", user.Fullname);
                LogContext.PushProperty("role", user.Role);
            }

            if (httpContext.Connection.RemoteIpAddress != null)
                LogContext.PushProperty("ipAddress", httpContext.Connection.RemoteIpAddress);

            LogContext.PushProperty("headers", httpContext.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), destructureObjects: true);
            LogContext.PushProperty("schema", httpContext.Request.Scheme);
            LogContext.PushProperty("host", httpContext.Request.Host);
            LogContext.PushProperty("path", httpContext.Request.Path);
            LogContext.PushProperty("method", httpContext.Request.Method);
            LogContext.PushProperty("queryString", httpContext.Request.QueryString);

            var body = await GetRequestBody(httpContext.Request);
            if (!string.IsNullOrEmpty(body))
                LogContext.PushProperty("body", body);

            var stopwatch = Stopwatch.StartNew();
            await _next(httpContext);
            stopwatch.Stop();

            LogContext.PushProperty("duration", stopwatch.Elapsed.TotalSeconds);

            Log.Information("[{RequestMethod}] {RequestPath}", httpContext.Request.Method, httpContext.Request.Path);
        }

        private async Task<string> GetRequestBody(HttpRequest request)
        {
            request.EnableBuffering();
            var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Position = 0;
            return requestBody;
        }
    }
}

