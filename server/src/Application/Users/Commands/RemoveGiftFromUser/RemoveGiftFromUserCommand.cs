using MediatR;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Application.Exceptions;

namespace Prezentex.Application.Users.Commands.RemoveGiftFromUser
{
    public record RemoveGiftFromUserCommand : IRequest<Unit>
    {
        public RemoveGiftFromUserCommand(Guid giftId, Guid requestedUserId, Guid userId)
        {
            GiftId = giftId;
            RequestedUserId = requestedUserId;
            UserId = userId;
        }

        public Guid GiftId { get; set; }
        public Guid RequestedUserId { get; set; }
        public Guid UserId { get; set; }

    }
    public class RemoveGiftFromUserHandler : IRequestHandler<RemoveGiftFromUserCommand, Unit>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IGiftsRepository _giftsRepository;

        public RemoveGiftFromUserHandler(IUsersRepository usersRepository, IGiftsRepository giftsRepository)
        {
            _usersRepository = usersRepository;
            _giftsRepository = giftsRepository;
        }
        public async Task<Unit> Handle(RemoveGiftFromUserCommand request, CancellationToken cancellationToken)
        {
            var isCorrectUser = request.RequestedUserId == request.UserId;
            if (!isCorrectUser)
                throw new UnauthorizedAccessException();

            var user = await _usersRepository.GetUserAsync(request.RequestedUserId);
            var gift = await _giftsRepository.GetGiftAsync(request.GiftId);

            if (user == null || gift == null || !user.Gifts.Any(gift => gift.Id == request.GiftId))
                throw new ResourceNotFoundException();

            var userOwnsGift = gift.UserId == user.Id;
            if (!userOwnsGift)
                throw new ArgumentException("User does not own this gift");

            await _usersRepository.RemoveGiftFromUserAsync(request.UserId, request.GiftId);

            return Unit.Value;

        }
    }
}
