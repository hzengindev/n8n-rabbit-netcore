using System;
using System.Text.Json.Serialization;

namespace DeveloperAPI.Core.Domain.Entities
{
	public class Project
	{
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("languages")]

        public string Languages { get; set; }
    }
}

