namespace Prezentex.Api.Settings
{
    public class CosmosSettings
    {
        public string AccountEndpoint { get; set; }
        public string AccountKey { get; set; }
        public string Database { get; set; }

        public string ConnectionString
        {
            get
            {
                return $"AccountEndpoint={AccountEndpoint};AccountKey={AccountKey}";
            }
        }
    }
}
