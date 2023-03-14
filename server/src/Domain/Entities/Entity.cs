using System.ComponentModel.DataAnnotations;

namespace Prezentex.Domain.Entities
{
    public abstract class Entity
    {
        [Key]
        public Guid Id { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}