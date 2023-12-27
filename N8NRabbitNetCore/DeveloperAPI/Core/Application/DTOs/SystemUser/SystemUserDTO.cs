using System;
using System.Text.Json.Serialization;
using DeveloperAPI.Core.Domain.Entities;

namespace DeveloperAPI.Core.Application.DTOs.SystemUser
{
	public class SystemUserDTO
	{
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("fullname")]
        public string Fullname { get; set; }
        [JsonPropertyName("role")]
        public SystemUserRole Role { get; set; }

        public SystemUserDTO()
        {

        }

        public SystemUserDTO(Domain.Entities.SystemUser user)
        {
            Id = user.Id;
            Username = user.Username;
            Fullname = user.Fullname;
            Role = user.Role;
        }
    }
}

