using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainAPI.Services.N8N;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MainAPI.Controllers
{
    [ApiController]
    [Route("api/developer")]
    public class DeveloperController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        private readonly IN8NAPI _iN8NAPI;

        public DeveloperController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IN8NAPI iN8NAPI)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _iN8NAPI = iN8NAPI;
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> Get([FromRoute] string username)
        {
            //var httpClient = _httpClientFactory.CreateClient();
            //httpClient.BaseAddress = new Uri(_configuration.GetValue<string>("n8n:baseURL"));
            //await httpClient.GetAsync($"webhook/62f43830-4bb6-4c5e-a290-5fe80b0a8aca?username={username}");
            //return Ok($"triggered for {username}");

            return Ok(await _iN8NAPI.DeveloperAnalyze(new Services.N8N.DTOs.DeveloperAnalyzeQuery() { Username = username }));
        }
    }
}

