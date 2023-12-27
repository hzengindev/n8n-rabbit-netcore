using System;
using System.Text.Json.Serialization;

namespace Analyzer.Services.Developer.DTOs
{
	public class DeveloperDetailResponse
	{
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("fullname")]
        public string Fullname { get; set; }
        [JsonPropertyName("projects")]
        public List<Project> Projects { get; set; }
    }

    public class Project
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("languages")]
        public string Languages { get; set; }
    }
}

