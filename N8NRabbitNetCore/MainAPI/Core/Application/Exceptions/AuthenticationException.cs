using System;
namespace MainAPI.Core.Application.Exceptions
{
    public class AuthenticationException : Exception
    {
        public int? StatusCode { get; set; }

        public AuthenticationException(string message) : base(message)
        {
            StatusCode = 401;
        }

        public AuthenticationException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}

