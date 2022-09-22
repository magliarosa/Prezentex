namespace Prezentex.Api.Entities
{
    public class Recipient : Entity
    {
        public string? Name { get; set; }
        public string? Note { get; set; }
        public IList<Gift> Gifts { get; set; } = new List<Gift>();
        public DateTimeOffset BirthDay { get; set; }
        public DateTimeOffset NameDay { get; set; }

    }
}