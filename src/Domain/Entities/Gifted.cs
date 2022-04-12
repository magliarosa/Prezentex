using Prezentex.Domain.Common;
using Prezentex.Domain.ValueObjects;
using System;

namespace Prezentex.Domain.Entities;

public class Gifted : AuditableEntity
{
    public int Id { get; set; }

    public string? Name { get; set; }
    public DateTime BirthDay { get; set; }
    public DateTime NameDay { get; set; }
    public IEnumerable<Gift>? Gifts { get; set; }
}
