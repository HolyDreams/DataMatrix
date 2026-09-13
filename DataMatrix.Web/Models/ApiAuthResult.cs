using DataMatrix.Web.Enums;
using System.Net;

namespace DataMatrix.Web.Models
{
    public class ApiAuthResult
    {
        public ApiAuthResult() { }

        public bool IsSuccess { get; private set; }

        public HttpStatusCode ResponseCode { get; private set; } = HttpStatusCode.InternalServerError;

        public string ErrorMessage { get; private set; } = "Неизвестная ошибка";

        public List<Role> Roles { get; private set; } = [];

        public string Cookie { get; private set; } = "";

        public static ApiAuthResult Success(List<Role> roles, string cookie) => new() { IsSuccess = true, Cookie = cookie, Roles = roles };

        public static ApiAuthResult Failure(string errorMessage, HttpStatusCode statusCode) => new() { IsSuccess = false, ErrorMessage = errorMessage, ResponseCode = statusCode };
    }
}
