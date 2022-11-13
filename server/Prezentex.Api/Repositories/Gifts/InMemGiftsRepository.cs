using Prezentex.Api.Entities;

namespace Prezentex.Api.Repositories
{
    public class InMemGiftsRepository : IGiftsRepository
    {
        private readonly List<Gift> gifts = new()
        {
            new Gift { Id = Guid.NewGuid(), Name = "Samsung TV", CreatedDate = DateTimeOffset.Now, UpdatedDate = DateTimeOffset.Now, Description = "New 65' with smart TV", Price = 2500, ProductUrl = "wp.pl" },
            new Gift { Id = Guid.NewGuid(), Name = "Road Bike", CreatedDate = DateTimeOffset.Now, UpdatedDate = DateTimeOffset.Now, Description = "Colnago with Campagnolo Record Groupset", Price = 15000, ProductUrl = "allegro.pl" },
        };


        public async Task CreateGiftAsync(Gift gift)
        {
            gifts.Add(gift);
            await Task.CompletedTask;
        }

        public async Task DeleteGiftAsync(Guid id)
        {
            var index = gifts.FindIndex(x => x.Id == id);
            if (index >= 0)
                gifts.RemoveAt(index);
            
            await Task.CompletedTask;
        }

        public async Task<Gift> GetGiftAsync(Guid id)
        {
            var gift = gifts.Where(g => g.Id == id).SingleOrDefault();
            return await Task.FromResult(gift);
        }

        public async Task<IEnumerable<Gift>> GetGiftsAsync()
        {
            return await Task.FromResult(gifts);
        }

        public async Task UpdateGiftAsync(Gift gift)
        {
            var index = gifts.FindIndex(giftToChange => giftToChange.Id == gift.Id);
            if (index >= 0)
                gifts[index] = gift;

            await Task.CompletedTask;
        }
        
        public Task RemoveRecipientFromGiftAsync(Guid giftId, Guid recipientId)
        {
            throw new NotImplementedException();
        }
        
        public Task AddRecipientToGiftAsync(Guid giftId, Guid recipientId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserOwnsGiftAsync(Guid giftId, Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
