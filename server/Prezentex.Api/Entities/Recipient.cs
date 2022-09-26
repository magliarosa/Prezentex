namespace Prezentex.Api.Entities
{
    public class Recipient : Entity
    {
        public string? Name { get; set; }
        public string? Note { get; set; }
        public DateTimeOffset BirthDay { get; set; }
        public DateTimeOffset NameDay { get; set; }

    }
}