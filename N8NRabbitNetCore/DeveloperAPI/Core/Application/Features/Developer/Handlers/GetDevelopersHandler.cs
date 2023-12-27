using System;
using MediatR;
using DeveloperAPI.Core.Application.DTOs.Developer;
using DeveloperAPI.Core.Application.Features.Developer.Queries;

namespace DeveloperAPI.Core.Application.Features.Developer.Handlers
{
    public class GetDevelopersHandler : IRequestHandler<GetDevelopersQuery, List<DeveloperDTO>>
    {
        public async Task<List<DeveloperDTO>> Handle(GetDevelopersQuery request, CancellationToken cancellationToken)
        {
            var result = Data.Data.DB.Developers.Select(z => new DeveloperDTO()
            {
                Username = z.Username,
                Fullname = z.Fullname,
                Projects = z.Projects.Select(a => new DTOs.Developer.ProjectDTO()
                {
                    Name = a.Name,
                    Languages = a.Languages
                }).ToList()
            }).ToList();

            return await Task.FromResult(result);
        }
    }
}

