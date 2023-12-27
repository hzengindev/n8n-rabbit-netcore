using System;
using Refit;

namespace Analyzer.Services.Developer.DTOs
{
	public class DeveloperDetailQuery
	{
		[AliasAs("username")]
		public string Username { get; set; }
	}
}

