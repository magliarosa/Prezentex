﻿using Prezentex.Domain.Entities;

namespace Prezentex.Application.Common.Interfaces.Repositories
{
    public interface IGiftsRepository
    {
        Task<Gift> GetGiftAsync(Guid id);
        Task<IEnumerable<Gift>> GetGiftsAsync();
        Task CreateGiftAsync(Gift gift);
        Task UpdateGiftAsync(Gift gift);
        Task DeleteGiftAsync(Guid id);
        Task AddRecipientToGiftAsync(Guid giftId, Guid recipientId);
        Task RemoveRecipientFromGiftAsync(Guid giftId, Guid recipientId);
        Task<bool> UserOwnsGiftAsync(Guid giftId, Guid userId);
    }
}
