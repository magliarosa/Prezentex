using MediatR;
using Prezentex.Api.Commands;
using Prezentex.Api.Exceptions;
using Prezentex.Api.Repositories;

namespace Prezentex.Api.Handlers
{
    public class DeleteGiftHandler : IRequestHandler<DeleteGiftCommand, Unit>
    {
        private readonly IGiftsRepository _giftsRepository;

        public DeleteGiftHandler(IGiftsRepository giftsRepository)
        {
            _giftsRepository = giftsRepository;
        }
        public async Task<Unit> Handle(DeleteGiftCommand request, CancellationToken cancellationToken)
        {
            var existingGift = await _giftsRepository.GetGiftAsync(request.GiftId);
            if (existingGift == null)
                throw new ResourceNotFoundException();

            var userOwnsGift = existingGift.UserId == request.UserId;
            if (!userOwnsGift)
                throw new ArgumentException("You do not own this gift" );

            await _giftsRepository.DeleteGiftAsync(request.GiftId);

            return Unit.Value;
        }
    }
}
