using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DeveloperAPI.Attributes;
using DeveloperAPI.Core.Application.Features.Developer.Queries;

namespace DeveloperAPI.Controllers
{
    [ApiController]
    [Route("api/developers")]
    public class DeveloperController : ControllerBase
    {
        private readonly ILogger<DeveloperController> _logger;
        private readonly IMediator _mediator;

        public DeveloperController(ILogger<DeveloperController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }


        [APIAuthorizeFilter(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var result = await _mediator.Send(new GetDevelopersQuery());
            return Ok(result);
        }

        [APIAuthorizeFilter(Roles = "Admin")]
        [HttpGet("username/{username}")]
        public async Task<IActionResult> Get([FromRoute] string username)
        {
            // timeout sample
            if (!string.IsNullOrEmpty(username) && username.Equals("hzengindev2"))
            {
                await Task.Delay(1000);
                return new StatusCodeResult(408);
            }
                
            var result = await _mediator.Send(new GetDeveloperByUsernameQuery(username));
            return Ok(result);
        }
    }
}

