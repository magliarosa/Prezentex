using MediatR;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Application.Exceptions;

namespace Prezentex.Application.Users.Commands.AddGiftToUser
{
    public record AddGiftToUserCommand : IRequest<Unit>
    {
        public AddGiftToUserCommand(Guid giftId, Guid requestedUserId, Guid userId)
        {
            GiftId = giftId;
            RequestedUserId = requestedUserId;
            UserId = userId;
        }

        public Guid GiftId { get; set; }
        public Guid RequestedUserId { get; set; }
        public Guid UserId { get; set; }
    }
    public class AddGiftToUserHandler : IRequestHandler<AddGiftToUserCommand, Unit>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IGiftsRepository _giftsRepository;

        public AddGiftToUserHandler(IUsersRepository usersRepository, IGiftsRepository giftsRepository)
        {
            _usersRepository = usersRepository;
            _giftsRepository = giftsRepository;
        }
        public async Task<Unit> Handle(AddGiftToUserCommand request, CancellationToken cancellationToken)
        {
            var getUserTask = _usersRepository.GetUserAsync(request.UserId);
            var getGiftTask = _giftsRepository.GetGiftAsync(request.GiftId);
            await Task.WhenAll(getUserTask, getGiftTask);
            var user = getUserTask.Result;
            var gift = getGiftTask.Result;

            if (user == null || gift == null)
                throw new ResourceNotFoundException();

            var isCorrectUser = user.Id == request.UserId;
            if (!isCorrectUser)
                throw new UnauthorizedAccessException();

            var userOwnsGift = gift.UserId == user.Id;
            if (!userOwnsGift)
                throw new ArgumentException("User does not own this gift");

            await _usersRepository.AddGiftToUserAsync(request.RequestedUserId, request.GiftId);

            return Unit.Value;
        }
    }
}
