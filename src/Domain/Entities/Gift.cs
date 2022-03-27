using Prezentex.Domain.Common;
using Prezentex.Domain.ValueObjects;
using System.Collections.Generic;

namespace Prezentex.Domain.Entities;

public class Gift : AuditableEntity
{
    public int Id { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Url { get; set; }
}
