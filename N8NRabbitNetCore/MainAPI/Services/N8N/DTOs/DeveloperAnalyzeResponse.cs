using System;
using System.Text.Json.Serialization;

namespace MainAPI.Services.N8N.DTOs
{
	public class DeveloperAnalyzeResponse
	{
        [JsonPropertyName("message")]
        public string Message { get; set; }
	}
}

