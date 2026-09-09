using System.Text.Json.Serialization;

namespace DataMatrix.Web.HttpClients.Clients.Api.Models
{
    public class GetAllCodesResponseDTO : BaseResponseDTO
    {
        public List<CodeDTO> Codes { get; set; } = [];
    }

    public class CodeDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        public required string Code { get; set; }

        [JsonPropertyName("author")]
        public string? Author { get; set; }
    }
}
