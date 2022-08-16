using Prezentex.Entities;

namespace Prezentex.Repositories
{
    public class InMemGiftsRepository : IGiftsRepository
    {
        private readonly List<Gift> gifts = new()
        {
            new Gift { Id = Guid.NewGuid(), Name = "Samsung TV", CreatedDate = DateTimeOffset.Now, UpdatedDate = DateTimeOffset.Now, Description = "New 65' with smart TV", Price = 2500, ProductUrl = "wp.pl" },
            new Gift { Id = Guid.NewGuid(), Name = "Road Bike", CreatedDate = DateTimeOffset.Now, UpdatedDate = DateTimeOffset.Now, Description = "Colnago with Campagnolo Record Groupset", Price = 15000, ProductUrl = "allegro.pl" },
        };

        public void CreateGift(Gift gift)
        {
            gifts.Add(gift);
        }

        public void DeleteGift(Guid id)
        {
            var index = gifts.FindIndex(x => x.Id == id);
            if (index >= 0)
                gifts.RemoveAt(index);
        }

        public Gift GetGift(Guid id)
        {
            var gift = gifts.Where(g => g.Id == id).SingleOrDefault();
            return gift;
        }

        public IEnumerable<Gift> GetGifts()
        {
            return gifts;
        }

        public void UpdateGift(Gift gift)
        {
            var index = gifts.FindIndex(giftToChange => giftToChange.Id == gift.Id);
            if (index >= 0)
                gifts[index] = gift;
        }
    }
}
