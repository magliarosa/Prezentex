using MediatR;
using Prezentex.Api.Commands;
using Prezentex.Api.Exceptions;
using Prezentex.Api.Repositories;
using Prezentex.Api.Repositories.Recipients;

namespace Prezentex.Api.Handlers
{
    public class RemoveRecipientFromGiftHandler : IRequestHandler<RemoveRecipientFromGiftCommand, Unit>
    {
        private readonly IGiftsRepository _giftsRepository;
        private readonly IRecipientsRepository _recipientsRepository;

        public RemoveRecipientFromGiftHandler(IGiftsRepository giftsRepository, IRecipientsRepository recipientsRepository)
        {
            _giftsRepository = giftsRepository;
            _recipientsRepository = recipientsRepository;
        }
        public async Task<Unit> Handle(RemoveRecipientFromGiftCommand request, CancellationToken cancellationToken)
        {
            var gift = await _giftsRepository.GetGiftAsync(request.GiftId);
            var recipientId = request.RecipientId;
            var recipient = await _recipientsRepository.GetRecipientAsync(recipientId);

            if (gift == null || recipient == null || !gift.Recipients.Any(recipient => recipient.Id == recipientId))
                throw new ResourceNotFoundException();

            var userOwnsGift = gift.UserId == request.UserId;
            if (!userOwnsGift)
                throw new ArgumentException("You do not own this gift" );

            var userOwnsRecipient = recipient.UserId == request.UserId;
            if (!userOwnsRecipient)
                throw new ArgumentException("You do not own this recipient" );


            await _giftsRepository.RemoveRecipientFromGiftAsync(gift.Id, recipientId);

            return Unit.Value;
        }
    }
}
