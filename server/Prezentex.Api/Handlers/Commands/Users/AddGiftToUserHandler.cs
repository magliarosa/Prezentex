using MediatR;
using Microsoft.Azure.Cosmos;
using Prezentex.Api.Commands.Users;
using Prezentex.Api.Dtos;
using Prezentex.Api.Entities;
using Prezentex.Api.Exceptions;
using Prezentex.Api.Repositories;

namespace Prezentex.Api.Handlers.Commands.Users
{
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
