using System;
using MediatR;
using DeveloperAPI.Core.Application.DTOs.Developer;

namespace DeveloperAPI.Core.Application.Features.Developer.Queries
{
	public class GetDevelopersQuery : IRequest<List<DeveloperDTO>>
    {
	}
}

