using System;
using MediatR;
using DeveloperAPI.Core.Application.DTOs.Authentication;

namespace DeveloperAPI.Core.Application.Features.Authentication.Commands
{
    public class RefreshAuthenticationCommand : IRequest<AuthenticationTokenDto>
    {
        public string RefreshToken { get; set; }
    }
}

