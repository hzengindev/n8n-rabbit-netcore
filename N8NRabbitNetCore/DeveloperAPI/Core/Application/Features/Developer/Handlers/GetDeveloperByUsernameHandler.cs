using System;
using MediatR;
using DeveloperAPI.Core.Application.DTOs.Developer;
using DeveloperAPI.Core.Application.Exceptions;
using DeveloperAPI.Core.Application.Features.Developer.Queries;

namespace DeveloperAPI.Core.Application.Features.Developer.Handlers
{
    public class GetDeveloperByUsernameHandler : IRequestHandler<GetDeveloperByUsernameQuery, DeveloperDTO>
    {
        public async Task<DeveloperDTO> Handle(GetDeveloperByUsernameQuery request, CancellationToken cancellationToken)
        {
            var user = Data.Data.DB.Developers.FirstOrDefault(z => z.Username == request.Username);

            if (user is null)
                throw new RecordNotFoundException($"There is not user for {request.Username}");

            var result = new DeveloperDTO()
            {
                Username = user.Username,
                Fullname = user.Fullname,
                Projects = user.Projects.Select(a => new DTOs.Developer.ProjectDTO()
                {
                    Name = a.Name,
                    Languages = a.Languages
                }).ToList()
            };

            return await Task.FromResult(result);
        }
    }
}

