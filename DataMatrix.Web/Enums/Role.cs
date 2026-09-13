namespace DataMatrix.Web.Enums
{
    public enum Role
    {
        Unknown,
        Viewer,
        Creator
    }

    public static class Roles
    {
        public const string Creator = "Создатель";
        public const string Viewer = "Смотритель";
        public const string Unknown = "Неизвестно";

        public static string GetRole(this Role role)
        {
            return role switch
            {
                Role.Creator => Creator,
                Role.Viewer => Viewer,
                _ => Unknown,
            };
        }

        public static Role ToRole(this string role)
        {
            foreach (var r in Enum.GetValues<Role>())
                if (string.Equals(role, r.GetRole(), StringComparison.OrdinalIgnoreCase))
                    return r;

            return Role.Unknown;
        }
    }
}
