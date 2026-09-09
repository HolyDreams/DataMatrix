using DataMatrix.Web.Enums;
using DataMatrix.Web.HttpClients.Clients.Api.Models;
using DataMatrix.Web.HttpClients.Interfaces;
using DataMatrix.Web.Models;
using DataMatrix.Web.Models.Settings;
using System.Text.Json;

namespace DataMatrix.Web.HttpClients.Clients.Api
{
    public class ApiHttpClient : IApiHttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly FileSettings _fileSettings;

        public ApiHttpClient(IHttpClientFactory httpClientFactory, FileSettings fileSettings)
        {
            _httpClientFactory = httpClientFactory;
            _fileSettings = fileSettings;
        }

        public async Task<string> AuthAsync(string login, string password)
        {
            using var client = GetHttpClient();
            var request = new LoginRequestDTO(login, password);

            var response = await client.PostAsJsonAsync("auth/login", request);
            var respContent = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(string.Join("\r\n", respContent!.Errors ?? []), null, response.StatusCode);

            return GetCookie(response);
        }

        public async Task LogoutAsync(string cookie)
        {
            using var client = GetHttpClient(cookie);
            var resp = await client.PostAsync("auth/logout", null);
            var body = await resp.Content.ReadAsStringAsync();
        }

        public async Task<string> RegisterAsync(string login, string password, params Role[] roles)
        {
            using var client = GetHttpClient();
            var registerRequest = new RegisterRequestDTO(login, password, roles.Select(r => r.ToString()));

            var response = await client.PostAsJsonAsync("auth/register", registerRequest);
            var respContent = await response.Content.ReadFromJsonAsync<RegisterResponseDTO>();
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(string.Join("\r\n", respContent?.Errors ?? []), null, response.StatusCode);

            return GetCookie(response);
        }

        public async Task CreateRandomAsync(string cookie)
        {
            using var client = GetHttpClient(cookie);

            var response = await client.GetAsync("DataMatrix/GenerateRandom");
            var contentString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(contentString, null, response.StatusCode);
        }

        public async Task<List<CodeDTO>> GetAllAsync(string cookie)
        {
            using var client = GetHttpClient(cookie);

            var response = await client.GetAsync("DataMatrix/GetAllCodes");
            var contentString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(contentString, null, response.StatusCode);
            if (string.IsNullOrWhiteSpace(contentString))
                return [];

            return [.. JsonSerializer.Deserialize<List<CodeDTO>>(contentString) ?? []];
        }

        public async Task<FileModel> Download(int id, string cookie)
        {
            using var client = GetHttpClient(cookie);

            var response = await client.GetAsync($"DataMatrix/PrintCodeImageById/{id}");
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(await response.Content.ReadAsStringAsync(), null, response.StatusCode);

            using var downloadStream = await response.Content.ReadAsStreamAsync();
            return new FileModel(_fileSettings.Directory, downloadStream);
        }

        private HttpClient GetHttpClient(string cookie = "")
        {
            var client = _httpClientFactory.CreateClient(nameof(ApiHttpClient));
            if (!string.IsNullOrWhiteSpace(cookie))
            {
                client.DefaultRequestHeaders.Add("Cookie", cookie);

            }
            return client;
        }

        private string GetCookie(HttpResponseMessage response)
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
