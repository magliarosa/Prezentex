using Prezentex.Domain.Common;

namespace Prezentex.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}
