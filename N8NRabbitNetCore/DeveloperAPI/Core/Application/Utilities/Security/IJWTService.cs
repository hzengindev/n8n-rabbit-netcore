using System;
using System.IdentityModel.Tokens.Jwt;

namespace DeveloperAPI.Core.Application.Utilities.Security
{
    public interface IJWTService
    {
        JWTToken GenerateAccessToken(Guid userId);
        JWTToken GenerateRefreshToken(Guid userId);
        JwtSecurityToken GetJWTSecurityToken(string token);
    }
}

