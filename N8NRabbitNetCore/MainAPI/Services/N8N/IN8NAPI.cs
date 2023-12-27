using System;
using MainAPI.Services.N8N.DTOs;
using Refit;

namespace MainAPI.Services.N8N
{
	public interface IN8NAPI
	{
        [Get("/webhook/62f43830-4bb6-4c5e-a290-5fe80b0a8aca")]
        Task<DeveloperAnalyzeResponse> DeveloperAnalyze(DeveloperAnalyzeQuery query);
    }
}

