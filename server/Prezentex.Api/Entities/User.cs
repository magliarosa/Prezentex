namespace Prezentex.Api.Entities
{
    public class User : Entity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public ICollection<Gift> Gifts { get; set; } = new List<Gift>();
        public ICollection<Recipient> Recipients { get; set; } = new List<Recipient>();
    }
}
