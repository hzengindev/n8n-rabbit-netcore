using System;
using System.Text.Json.Serialization;

namespace Analyzer.Models
{
    public class Project
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("languages")]
        public string Languages { get; set; }
    }
}

