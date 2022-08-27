namespace Prezentex.Api.Entities
{
    public abstract record Entity
    {
        public Guid Id { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
