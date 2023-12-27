using System;
using System.Text.Json.Serialization;

namespace DeveloperAPI.Core.Application.DTOs.Developer
{
	public class ProjectDTO
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("languages")]
        public string Languages { get; set; }
    }
}

