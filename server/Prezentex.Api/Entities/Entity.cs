using System.ComponentModel.DataAnnotations;

namespace Prezentex.Api.Entities
{
    public abstract class Entity
    {
        [Key]
        public Guid Id { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}