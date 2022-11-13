using System.ComponentModel.DataAnnotations.Schema;

namespace Prezentex.Api.Entities
{
    public class Gift : Entity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ProductUrl { get; set; }
        public ICollection<Recipient> Recipients { get; set; } = new List<Recipient>();
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}