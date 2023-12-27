using System;
namespace DeveloperAPI.Core.Application.DTOs.Authentication
{
    public class AuthenticationTokenDto
    {
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpirationDate { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpirationDate { get; set; }
    }
}

