namespace Prezentex.Settings
{
    public class PostgresSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }

        public string ConnectionString
        {
            get
            {
                return $"Server={Host};Database={Database};Port={Port};User Id={User};Password={Password}";
            }
        }
    }
}
