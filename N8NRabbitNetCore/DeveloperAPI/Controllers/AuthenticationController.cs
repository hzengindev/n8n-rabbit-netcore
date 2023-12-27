using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DeveloperAPI.Core.Application.Features.Authentication.Commands;

namespace DeveloperAPI.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IMediator _mediator;

        public AuthenticationController(ILogger<AuthenticationController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }


        [HttpPost("forms")]
        public async Task<IActionResult> FormsAuthentication([FromBody] FormsAuthenticationCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAuthentication([FromBody] RefreshAuthenticationCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}

