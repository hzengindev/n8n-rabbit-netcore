using System;
using System.Text.Json.Serialization;

namespace DeveloperAPI.Core.Domain.Entities
{
    public class SystemUser
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
        [JsonPropertyName("fullname")]
        public string Fullname { get; set; }
        [JsonPropertyName("role")]
        public SystemUserRole Role { get; set; }
    }

    public enum SystemUserRole : short
    {
        Admin = 1,
        Member = 2
    }
}

