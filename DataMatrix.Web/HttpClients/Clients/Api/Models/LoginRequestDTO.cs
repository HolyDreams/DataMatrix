namespace DataMatrix.Web.HttpClients.Clients.Api.Models
{
    /// <summary>
    /// Модель авторизации
    /// </summary>
    public class LoginRequestDTO
    {
        public LoginRequestDTO() { }

        public LoginRequestDTO(string name, string password)
        {
            Name = name;
            Password = password;
        }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }
    }
}
