using System;
using MediatR;
using DeveloperAPI.Core.Application.DTOs.Developer;

namespace DeveloperAPI.Core.Application.Features.Developer.Queries
{
	public class GetDeveloperByUsernameQuery : IRequest<DeveloperDTO>
    {
        public GetDeveloperByUsernameQuery(string username)
        {
            Username = username;
        }

        public string Username { get; set; }
	}
}

