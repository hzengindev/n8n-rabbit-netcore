using System;
using DeveloperAPI.Core.Domain.Entities;

namespace DeveloperAPI.Core.Application.Utilities.Security
{
    public class CurrentUser : ICurrentUser
    {
        public Guid Id { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }
        public SystemUserRole Role { get; set; }
    }
}

