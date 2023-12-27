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
    public class FormsAuthenticationHandler : IRequestHandler<FormsAuthenticationCommand, AuthenticationTokenDto>
    {
        private readonly IJWTService _jwtService;
        private readonly IDistributedCache _distributedCache;
        private readonly IConfiguration _configuration;

        public FormsAuthenticationHandler(IJWTService jwtService, IDistributedCache distributedCache, IConfiguration configuration)
        {
            _jwtService = jwtService;
            _distributedCache = distributedCache;
            _configuration = configuration;
        }

        public async Task<AuthenticationTokenDto> Handle(FormsAuthenticationCommand request, CancellationToken cancellationToken)
        {
            var user = Data.Data.DB.SystemUsers.FirstOrDefault(z => z.Username == request.Username && z.Password == request.Password);
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

            return await Task.FromResult(result);
        }
    }
}

