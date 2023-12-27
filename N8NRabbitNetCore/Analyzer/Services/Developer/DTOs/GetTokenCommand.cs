using System;
using Refit;

namespace Analyzer.Services.Developer.DTOs
{
	public class GetTokenCommand
	{
		[AliasAs("username")]
		public string Username { get; set; }
        [AliasAs("password")]
        public string Password { get; set; }
	}
}

