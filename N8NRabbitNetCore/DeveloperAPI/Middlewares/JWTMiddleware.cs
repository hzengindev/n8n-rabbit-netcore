using System;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using DeveloperAPI.Core.Application.DTOs.SystemUser;
using DeveloperAPI.Core.Application.Utilities.Security;

namespace DeveloperAPI.Middlewares
{
    public class JWTMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JWTMiddleware> _logger;
        private readonly IJWTService _jwtService;
        private readonly IDistributedCache _distributedCache;

        public JWTMiddleware(RequestDelegate next, ILogger<JWTMiddleware> logger, IDistributedCache distributedCache, IJWTService jwtService)
        {
            _next = next;
            _logger = logger;
            _jwtService = jwtService;
            _distributedCache = distributedCache;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token == null)
            {
                await _next(context);
            }
            else
            {
                var result = await AttachUserToContext(context, token);
                if (result)
                    await _next(context);
            }
        }

        private async Task<bool> AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var jwtToken = _jwtService.GetJWTSecurityToken(token);
                var sessionId = jwtToken.Claims.First(z => z.Type == CustomClaimTypes.SessionId).Value;

                var session = await _distributedCache.GetStringAsync($"DataverseManager:Session:{sessionId}");
                if (string.IsNullOrEmpty(session))
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        success = false,
                        message = "unauthorized",
                        code = StatusCodes.Status401Unauthorized
                    }));
                    return false;
                }

                var user = JsonSerializer.Deserialize<SystemUserDTO>(session);
                context.Items["CurrentUser"] = user;
                return true;
            }
            catch (Exception ex)
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
                _logger.LogError(ex, ex.Message);
            }

            return true;
        }
    }
}

