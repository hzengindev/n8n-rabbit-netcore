using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DeveloperAPI.Core.Application.Utilities.Security
{
    public class JWTManager : IJWTService
    {
        private readonly IConfiguration _configuration;

        public JWTManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JWTToken GenerateAccessToken(Guid userId)
        {
            var sessionId = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            var expirationDate = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JWTOption:AccessTokenExpiration"]));
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWTOption:SecurityKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _configuration["JWTOption:Audience"],
                Issuer = _configuration["JWTOption:Issuer"],
                Expires = expirationDate,
                SigningCredentials = signingCredentials,
                IssuedAt = DateTime.UtcNow,
                Subject = new ClaimsIdentity()
            };

            tokenDescriptor.Subject.AddClaim(new Claim(CustomClaimTypes.SessionId, sessionId));
            tokenDescriptor.Subject.AddClaim(new Claim(CustomClaimTypes.UserId, userId.ToString()));

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return new JWTToken()
            {
                SessionId = sessionId,
                Token = token,
                ExpirationDate = expirationDate,
            };
        }

        public JWTToken GenerateRefreshToken(Guid userId)
        {
            var sessionId = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            var expirationDate = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JWTOption:RefreshTokenExpiration"]));
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWTOption:SecurityKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _configuration["JWTOption:Audience"],
                Issuer = _configuration["JWTOption:Issuer"],
                Expires = expirationDate,
                SigningCredentials = signingCredentials,
                IssuedAt = DateTime.UtcNow,
                Subject = new ClaimsIdentity()
            };

            tokenDescriptor.Subject.AddClaim(new Claim(CustomClaimTypes.SessionId, sessionId));
            tokenDescriptor.Subject.AddClaim(new Claim(CustomClaimTypes.UserId, userId.ToString()));

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return new JWTToken()
            {
                SessionId = sessionId,
                Token = token,
                ExpirationDate = expirationDate
            };
        }

        public JwtSecurityToken GetJWTSecurityToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidIssuer = _configuration["JWTOption:Issuer"],
                ValidAudience = _configuration["JWTOption:Audience"],
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWTOption:SecurityKey"])),
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            return jwtToken;
        }
    }

    public static class CustomClaimTypes
    {
        public const string SessionId = "SessionId";
        public const string UserId = "Id";
    }

    public class JWTToken
    {
        public string SessionId { get; internal set; }
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}

