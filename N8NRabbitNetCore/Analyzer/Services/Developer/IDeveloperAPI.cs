using System;
using Analyzer.Services.Developer.DTOs;
using Refit;

namespace Analyzer.Services.Developer
{
	public interface IDeveloperAPI
	{
        [Post("/api/authentication/forms")]
        Task<GetTokenResponse> GetToken(GetTokenCommand query);

        [Get("/api/developers/username/{query.username}")]
        [Headers("Authorization: Bearer")]
        Task<DeveloperDetailResponse> DeveloperDetail(DeveloperDetailQuery query);
    }
}