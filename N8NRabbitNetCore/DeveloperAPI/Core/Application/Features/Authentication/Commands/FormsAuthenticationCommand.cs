using System;
using MediatR;
using DeveloperAPI.Core.Application.DTOs.Authentication;

namespace DeveloperAPI.Core.Application.Features.Authentication.Commands
{
    public class FormsAuthenticationCommand : IRequest<AuthenticationTokenDto>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

