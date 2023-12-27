using System;
using System.Text.Json.Serialization;

namespace DeveloperAPI.Core.Application.DTOs.Developer
{
	public class DeveloperDTO
	{
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("fullname")]
        public string Fullname { get; set; }
        [JsonPropertyName("projects")]
        public List<ProjectDTO> Projects { get; set; }
    }
}