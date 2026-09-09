namespace DataMatrix.Web.HttpClients.Clients.Api.Models
{
    public class BaseResponseDTO
    {
        public string? Message { get; set; }

        public List<string>? Errors { get; set; }
    }
}
