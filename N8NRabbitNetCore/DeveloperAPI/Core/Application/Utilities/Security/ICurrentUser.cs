using System;
using DeveloperAPI.Core.Domain.Entities;

namespace DeveloperAPI.Core.Application.Utilities.Security
{
    public interface ICurrentUser
    {
        public Guid Id { get; }
        public string Fullname { get; }
        public string Username { get; }
        public SystemUserRole Role { get; }
    }
}