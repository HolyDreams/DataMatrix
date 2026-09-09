using DataMatrix.Web.Enums;
using DataMatrix.Web.HttpClients.Clients.Api.Models;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace DataMatrix.Web.Controllers.Home.Models
{
    public class HomeViewModel
    {
        private static readonly JsonSerializerOptions SerializerSettings = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public bool IsAuthorized { get; set; }

        public int CurrentPage { get; set; } = 0;

        public int TotalPages { get; set; } = 0;

        public int PageSize { get; set; }

        public List<CodeDTO> Codes { get; set; } = [];

        public Dictionary<int, string> Roles { get; init; } = Enum.GetValues(typeof(Role))
                                                                .Cast<Role>()
                                                                .ToDictionary(t => (int)t, t => t.ToString());

        public string ToJson() => JsonSerializer.Serialize(this, SerializerSettings);
        
    }
}
