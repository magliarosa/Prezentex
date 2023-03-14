using MediatR;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Application.Exceptions;

namespace Prezentex.Application.Gifts.Commands.AddRecipientToGift
{
    public record AddRecipientToGiftCommand : IRequest<Unit>
    {
        public AddRecipientToGiftCommand(Guid recipientId, Guid giftId, Guid userId)
        {
            RecipientId = recipientId;
            GiftId = giftId;
            UserId = userId;
        }

        public Guid RecipientId { get; set; }
        public Guid GiftId { get; set; }
        public Guid UserId { get; set; }

    }

    public class AddRecipientToGiftHandler : IRequestHandler<AddRecipientToGiftCommand, Unit>
    {
        private readonly IGiftsRepository _giftsRepository;
        private readonly IRecipientsRepository _recipientsRepository;

        public AddRecipientToGiftHandler(IGiftsRepository giftsRepository, IRecipientsRepository recipientsRepository)
        {
            _giftsRepository = giftsRepository;
            _recipientsRepository = recipientsRepository;
        }
        public async Task<Unit> Handle(AddRecipientToGiftCommand request, CancellationToken cancellationToken)
        {
            var gift = await _giftsRepository.GetGiftAsync(request.GiftId);
            var recipientId = request.RecipientId;
            var recipient = await _recipientsRepository.GetRecipientAsync(recipientId);

            if (gift == null || recipient == null)
                throw new ResourceNotFoundException();

            var userOwnsGift = gift.UserId == request.UserId;
            if (!userOwnsGift)
                throw new ArgumentException("You do not own this gift");

            var userOwnsRecipient = recipient.UserId == request.UserId;
            if (!userOwnsRecipient)
                throw new ArgumentException("You do not own this recipient");

            await _giftsRepository.AddRecipientToGiftAsync(gift.Id, recipientId);

            return Unit.Value;
        }
    }
}
