using System;
namespace DeveloperAPI.Core.Application.Exceptions
{
    public class AuthorizationException : Exception
    {
        public int? StatusCode { get; set; }

        public AuthorizationException(string message) : base(message)
        {
            StatusCode = 403;
        }

        public AuthorizationException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}

