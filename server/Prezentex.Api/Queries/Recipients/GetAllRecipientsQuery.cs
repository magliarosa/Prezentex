using MediatR;
using Prezentex.Api.Entities;

namespace Prezentex.Api.Queries.Recipients
{
    public class GetAllRecipientsQuery : IRequest<IEnumerable<Recipient>>
    {
        public GetAllRecipientsQuery() { }
    }
}
