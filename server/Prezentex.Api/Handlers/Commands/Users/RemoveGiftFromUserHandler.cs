using MediatR;
using Prezentex.Api.Commands.Users;
using Prezentex.Api.Dtos;
using Prezentex.Api.Exceptions;
using Prezentex.Api.Repositories;

namespace Prezentex.Api.Handlers.Commands.Users
{
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
