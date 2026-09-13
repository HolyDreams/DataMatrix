using DataMatrix.Web.Enums;
using DataMatrix.Web.Extensions;
using DataMatrix.Web.HttpClients.Interfaces;
using DataMatrix.Web.Models;
using DataMatrix.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Net;

namespace DataMatrix.Web.Services
{
    public class SecurityService : ISecurityService
    {
        public const string CookieName = "AuthToken";
        public const string GetTokenName = ".AspNetCore.Identity.Application";
        private const string GetExpireName = "expires";

        private readonly IApiHttpClient _apiHttpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SecurityService(IApiHttpClient apiHttpClient, IHttpContextAccessor httpContextAccessor)
        {
            _apiHttpClient = apiHttpClient ?? throw new ArgumentNullException(nameof(apiHttpClient));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<SecurityAuthResult> Auth(HttpContext context, string login, string password)
        {
            var authResult = await _apiHttpClient.AuthAsync(login, password);
            if (!authResult.IsSuccess)
                return SecurityAuthResult.Failure(authResult.ResponseCode, authResult.ErrorMessage);

            await SetCookie(context, authResult.Cookie, login, authResult.Roles);
            return SecurityAuthResult.Success(authResult.Roles);
        }

        public async Task<SecurityAuthResult> Register(HttpContext context, string login, string password, IEnumerable<Role> roles)
        {
            if (!roles.Any())
                return SecurityAuthResult.Failure(HttpStatusCode.InternalServerError, "Нельзя зарегистрировать пользователя без указания ролей.");

            var registerResult = await _apiHttpClient.RegisterAsync(login, password, roles);
            if (!registerResult.IsSuccess)
                return SecurityAuthResult.Failure(registerResult.ResponseCode, registerResult.ErrorMessage);

            await SetCookie(context, registerResult.Cookie, login, registerResult.Roles);
            return SecurityAuthResult.Success(registerResult.Roles);
        }

        public async Task Logout(HttpContext context)
        {
            var token = _httpContextAccessor.HttpContext?.User.FindFirst(SecurityService.CookieName);
            if (token is null)
                return;

            try
            {
                await _apiHttpClient.LogoutAsync();
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode != HttpStatusCode.Unauthorized)
                    throw;
            }
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private async Task SetCookie(HttpContext context, string responseCookie, string login, List<Role> roles)
        {
            var parts = responseCookie.Split(';', StringSplitOptions.TrimEntries);
            var token = parts.First(c => c.StartsWith(GetTokenName)).Split('=')[1];
            var expire = parts.First(c => c.StartsWith(GetExpireName)).Split(',')[1];

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, login),
                new(CookieName, token)
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r.GetRole())));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.Parse(expire)
                });
        }
    }
}
