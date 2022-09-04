namespace Prezentex.Api.Entities
{
    public abstract class Entity
    {
        public Guid Id { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
