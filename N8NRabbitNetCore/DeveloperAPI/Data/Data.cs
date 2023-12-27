using System;
using System.Text.Json.Serialization;
using DeveloperAPI.Core.Domain.Entities;

namespace DeveloperAPI.Data
{
	public class Data
	{
		public static DBModel DB = new DBModel();
	}

    public class DBModel
    {
        [JsonPropertyName("developers")]
        public List<Developer> Developers { get; set; }

        [JsonPropertyName("systemUsers")]
        public List<SystemUser> SystemUsers { get; set; }
    }
}

