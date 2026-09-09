namespace DataMatrix.Web.HttpClients.Clients.Api.Models
{
    /// <summary>
    /// Запрос регистрации пользователя
    /// </summary>
    public class RegisterRequestDTO
    {
        public RegisterRequestDTO() { }

        public RegisterRequestDTO(string name, string password, IEnumerable<string> roles)
        {
            Name = name;
            Password = password;
            Roles = [..roles];
        }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Роли
        /// </summary>
        public List<string> Roles { get; set; }
    }
}
