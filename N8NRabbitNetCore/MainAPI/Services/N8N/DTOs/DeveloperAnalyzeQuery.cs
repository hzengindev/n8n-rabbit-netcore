using System;
using System.Text.Json.Serialization;
using Refit;

namespace MainAPI.Services.N8N.DTOs
{
	public class DeveloperAnalyzeQuery
	{
        [AliasAs("username")]
        public string Username { get; set; }
	}
}

