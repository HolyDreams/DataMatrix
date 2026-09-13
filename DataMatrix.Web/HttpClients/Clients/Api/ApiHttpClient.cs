using DataMatrix.Web.Enums;
using DataMatrix.Web.HttpClients.Clients.Api.Models;
using DataMatrix.Web.HttpClients.Interfaces;
using DataMatrix.Web.Models;
using DataMatrix.Web.Models.Settings;
using DataMatrix.Web.Services;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataMatrix.Web.HttpClients.Clients.Api
{
    public class ApiHttpClient : IApiHttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly FileSettings _fileSettings;
        private readonly JsonSerializerOptions deserializerOptions;

        public ApiHttpClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, FileSettings fileSettings)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _fileSettings = fileSettings ?? throw new ArgumentNullException(nameof(fileSettings));
            deserializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        public async Task<ApiAuthResult> AuthAsync(string login, string password)
        {
            using var client = GetHttpClient();
            var request = new LoginRequestDTO(login, password);

            var response = await client.PostAsJsonAsync("auth/login", request);
            if (!response.IsSuccessStatusCode)
                return ApiAuthResult.Failure(await response.Content.ReadAsStringAsync(), response.StatusCode);

            var respContent = await response.Content.ReadFromJsonAsync<LoginResponseDTO>(deserializerOptions);
            return ApiAuthResult.Success(respContent?.Roles ?? [], GetCookie(response));
        }

        public async Task LogoutAsync()
        {
            using var client = GetHttpClient();
            var response = await client.PostAsync("auth/logout", null);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(body, null, response.StatusCode);
        }

        public async Task<ApiAuthResult> RegisterAsync(string login, string password, IEnumerable<Role> roles)
        {
            using var client = GetHttpClient();
            var registerRequest = new RegisterRequestDTO(login, password, roles.Select(r => r.ToString()));

            var response = await client.PostAsJsonAsync("auth/register", registerRequest);
            
            var body = await response.Content.ReadAsStringAsync();
            RegisterResponseDTO? respContent;
            if (!response.IsSuccessStatusCode)
            {
                if (body.Length == 0)
                    return ApiAuthResult.Failure("Неизвестная ошибка", response.StatusCode);
                try
                {
                    respContent = JsonSerializer.Deserialize<RegisterResponseDTO>(body, deserializerOptions);
                    if (respContent?.Errors?.Any(err => err.Contains("is already taken")) ?? false)
                        return ApiAuthResult.Failure("Пользователь с таким логином уже есть.", HttpStatusCode.Unauthorized);
                    return ApiAuthResult.Failure(respContent?.Errors?.Count > 0 ? string.Join("\r\n", respContent!.Errors!) : "Неизвестная ошибка", response.StatusCode);
                }
                catch (Exception)
                {
                    return ApiAuthResult.Failure("Неизвестная ошибка", response.StatusCode);
                }

            }
            respContent = JsonSerializer.Deserialize<RegisterResponseDTO>(body, deserializerOptions);
            return ApiAuthResult.Success(respContent?.Roles ?? [], GetCookie(response));
        }

        public async Task CreateRandomAsync()
        {
            using var client = GetHttpClient();

            var response = await client.GetAsync("DataMatrix/GenerateRandom");
            var contentString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(contentString, null, response.StatusCode);
        }

        public async Task<List<CodeDTO>> GetAllAsync()
        {
            using var client = GetHttpClient();

            var response = await client.GetAsync("DataMatrix/GetAllCodes");
            var contentString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(contentString, null, response.StatusCode);
            if (string.IsNullOrWhiteSpace(contentString))
                return [];

            return [.. JsonSerializer.Deserialize<List<CodeDTO>>(contentString) ?? []];
        }

        public async Task<FileModel> Download(int id)
        {
            using var client = GetHttpClient();

            var response = await client.GetAsync($"DataMatrix/PrintCodeImageById/{id}");
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(await response.Content.ReadAsStringAsync(), null, response.StatusCode);

            using var downloadStream = await response.Content.ReadAsStreamAsync();
            return new FileModel(_fileSettings.Directory, id, downloadStream, true);
        }

        private HttpClient GetHttpClient()
        {
            var client = _httpClientFactory.CreateClient(nameof(ApiHttpClient));
            var token = _httpContextAccessor.HttpContext?.User.FindFirst(SecurityService.CookieName)?.Value;
            if (!string.IsNullOrWhiteSpace(token))
                client.DefaultRequestHeaders.Add("Cookie", string.Join('=', SecurityService.GetTokenName, token));
            return client;
        }

        private static string GetCookie(HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("Set-Cookie", out var cookieHeaders))
            {
                if (cookieHeaders.Any())
                    return cookieHeaders.First();
            }
            return "";
        }
    }
}
