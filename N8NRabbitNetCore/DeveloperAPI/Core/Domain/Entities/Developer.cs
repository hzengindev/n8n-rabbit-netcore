using System;
using System.Text.Json.Serialization;

namespace DeveloperAPI.Core.Domain.Entities
{
	public class Developer
	{
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("fullname")]
        public string Fullname { get; set; }
        [JsonPropertyName("projects")]
        public List<Project> Projects { get; set; }
    }
}

