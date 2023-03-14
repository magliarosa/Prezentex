using MediatR;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Domain.Entities;

namespace Prezentex.Application.Gifts.Queries.GetAllGifts
{
    public record GetAllGiftsQuery : IRequest<IEnumerable<Gift>>
    {

    }

    public class GetAllGiftsHandler : IRequestHandler<GetAllGiftsQuery, IEnumerable<Gift>>
    {
        private readonly IGiftsRepository _giftsRepository;

        public GetAllGiftsHandler(IGiftsRepository giftsRepository)
        {
            _giftsRepository = giftsRepository;
        }
        public async Task<IEnumerable<Gift>> Handle(GetAllGiftsQuery request, CancellationToken cancellationToken)
        {
            var gifts = await _giftsRepository.GetGiftsAsync();
            return await Task.FromResult(gifts);
        }
    }
}
