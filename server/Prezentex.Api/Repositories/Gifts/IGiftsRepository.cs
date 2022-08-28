﻿using Prezentex.Api.Entities;

namespace Prezentex.Api.Repositories
{
    public interface IGiftsRepository
    {
        Task<Gift> GetGiftAsync(Guid id);
        Task<IEnumerable<Gift>> GetGiftsAsync();
        Task CreateGiftAsync(Gift gift);
        Task UpdateGiftAsync(Gift gift);
        Task DeleteGiftAsync(Guid id);

    }
}