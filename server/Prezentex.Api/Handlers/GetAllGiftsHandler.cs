using MediatR;
using Prezentex.Api.Dtos;
using Prezentex.Api.Entities;
using Prezentex.Api.Queries;
using Prezentex.Api.Repositories;

namespace Prezentex.Api.Handlers
{
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
