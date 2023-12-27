using System;
using Microsoft.AspNetCore.Mvc.Filters;
using DeveloperAPI.Core.Application.Exceptions;
using DeveloperAPI.Core.Application.Utilities.Security;

namespace DeveloperAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class APIAuthorizeFilter : Attribute, IAuthorizationFilter
    {
        private ICurrentUser _currentUser;
        public string Roles { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (Roles is null || !Roles.Any()) return;
            _currentUser = context.HttpContext.RequestServices.GetService<ICurrentUser>();

            if (_currentUser == null)
                throw new AuthorizationException("Unauthorized", StatusCodes.Status401Unauthorized);

            if (!Roles.Split(',').Any(z => z == _currentUser.Role.ToString()))
                throw new AuthorizationException("You do not have permission to endpoint", StatusCodes.Status403Forbidden);
        }
    }
}

