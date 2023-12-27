﻿using System;
namespace Analyzer.Services.Developer.DTOs
{
	public class GetTokenResponse
	{
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpirationDate { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpirationDate { get; set; }
    }
}

