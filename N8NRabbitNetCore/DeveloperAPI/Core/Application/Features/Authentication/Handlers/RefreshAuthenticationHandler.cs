using System;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using DeveloperAPI.Core.Application.DTOs.SystemUser;
using DeveloperAPI.Core.Application.DTOs.Authentication;
using DeveloperAPI.Core.Application.Exceptions;
using DeveloperAPI.Core.Application.Features.Authentication.Commands;
using DeveloperAPI.Core.Application.Utilities.Security;

namespace DeveloperAPI.Core.Application.Features.Authentication.Handlers
{
    public class RefreshAuthenticationHandler : IRequestHandler<RefreshAuthenticationCommand, AuthenticationTokenDto>
    {
        private readonly IJWTService _jwtService;
        private readonly IDistributedCache _distributedCache;
        private readonly IConfiguration _configuration;

        public RefreshAuthenticationHandler(IJWTService jwtService, IDistributedCache distributedCache, IConfiguration configuration)
        {
            _jwtService = jwtService;
            _distributedCache = distributedCache;
            _configuration = configuration;
        }

        public async Task<AuthenticationTokenDto> Handle(RefreshAuthenticationCommand request, CancellationToken cancellationToken)
        {

            var jwtToken = _jwtService.GetJWTSecurityToken(request.RefreshToken);
            var sessionId = jwtToken.Claims.First(z => z.Type == CustomClaimTypes.SessionId).Value;

            var session = await _distributedCache.GetStringAsync($"DataverseManager:RefreshToken:{sessionId}");
            if (string.IsNullOrEmpty(session))
                throw new AuthenticationException("There is no refresh token.");

            var user = Data.Data.DB.SystemUsers.FirstOrDefault(z => z.Id == JsonSerializer.Deserialize<SystemUserDTO>(session).Id);
            if (user is null)
                throw new RecordNotFoundException($"There is not user.");

            var accessToken = _jwtService.GenerateAccessToken(user.Id);
            var refreshToken = _jwtService.GenerateRefreshToken(user.Id);

            var result = new AuthenticationTokenDto()
            {
                AccessToken = accessToken.Token,
                AccessTokenExpirationDate = accessToken.ExpirationDate,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpirationDate = refreshToken.ExpirationDate
            };

            var userDTO = new SystemUserDTO(user);

            await _distributedCache.SetStringAsync($"DataverseManager:Session:{accessToken.SessionId}", JsonSerializer.Serialize(userDTO), new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JWTOption:AccessTokenExpiration"]))
            }, cancellationToken);

            await _distributedCache.SetStringAsync($"DataverseManager:RefreshToken:{refreshToken.SessionId}", JsonSerializer.Serialize(userDTO), new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JWTOption:RefreshTokenExpiration"]))
            }, cancellationToken);

            await _distributedCache.RemoveAsync($"DataverseManager:RefreshToken:{sessionId}", cancellationToken);

            return await Task.FromResult(result);
        }
    }
}

