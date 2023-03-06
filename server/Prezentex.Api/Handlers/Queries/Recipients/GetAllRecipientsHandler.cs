using MediatR;
using Prezentex.Api.Entities;
using Prezentex.Api.Queries.Recipients;
using Prezentex.Api.Repositories.Recipients;

namespace Prezentex.Api.Handlers.Queries.Recipients
{
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
