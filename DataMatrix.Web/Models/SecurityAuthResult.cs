using DataMatrix.Web.Enums;
using System.Net;

namespace DataMatrix.Web.Models
{
    public class SecurityAuthResult
    {
        public SecurityAuthResult() { }

        public bool IsSuccess { get; private set; }

        public HttpStatusCode ResponseCode { get; private set; }

        public string? ErrorMessage { get; private set; }

        public IEnumerable<Role> Roles { get; private set; }

        public static SecurityAuthResult Success(IEnumerable<Role> roles) => new() { IsSuccess = true, Roles = roles };

        public static SecurityAuthResult Failure(HttpStatusCode statusCode, string error) => new() { IsSuccess = false, ErrorMessage = error, ResponseCode = statusCode };
    }
}
