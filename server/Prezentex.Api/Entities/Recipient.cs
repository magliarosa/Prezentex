namespace Prezentex.Api.Entities
{
    public class Recipient : Entity
    {
        public string? Name { get; set; }
        public string? Note { get; set; }
        public IEnumerable<Gift> Gifts { get; set; } = Enumerable.Empty<Gift>();
        public DateTimeOffset BirthDay { get; set; }
        public DateTimeOffset NameDay { get; set; }

    }
}
