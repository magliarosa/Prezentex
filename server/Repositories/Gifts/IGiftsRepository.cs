using Prezentex.Entities;

namespace Prezentex.Repositories
{
    public interface IGiftsRepository
    {
        Gift GetGift(Guid id);
        IEnumerable<Gift> GetGifts();
        void CreateGift(Gift gift);
        void UpdateGift(Gift gift);
        void DeleteGift(Guid id);

    }
}
