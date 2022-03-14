using Prezentex.Application.Common.Interfaces;

namespace Prezentex.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}
