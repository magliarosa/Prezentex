using System.ComponentModel.DataAnnotations.Schema;

namespace Prezentex.Domain.Entities
{
    public class Recipient : Entity
    {
        public string? Name { get; set; }
        public string? Note { get; set; }
        public DateTimeOffset BirthDay { get; set; }
        public DateTimeOffset NameDay { get; set; }
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}