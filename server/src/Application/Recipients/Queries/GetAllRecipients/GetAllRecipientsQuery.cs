using MediatR;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Domain.Entities;

namespace Prezentex.Application.Recipients.Queries.GetAllRecipients
{
    public record GetAllRecipientsQuery : IRequest<IEnumerable<Recipient>>
    {
        public GetAllRecipientsQuery() { }
    }
    public class GetAllRecipientsHandler : IRequestHandler<GetAllRecipientsQuery, IEnumerable<Recipient>>
    {
        private readonly IRecipientsRepository _recipientsRepository;

        public GetAllRecipientsHandler(IRecipientsRepository recipientsRepository)
        {
            _recipientsRepository = recipientsRepository;
        }

        public async Task<IEnumerable<Recipient>> Handle(GetAllRecipientsQuery request, CancellationToken cancellationToken)
        {
            var recipients = await _recipientsRepository.GetRecipientsAsync();
            return recipients;

        }
    }
}
