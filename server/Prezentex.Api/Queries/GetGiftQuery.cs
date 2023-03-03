using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prezentex.Api.Dtos;
using Prezentex.Api.Entities;

namespace Prezentex.Api.Queries
{
    public class GetGiftQuery : IRequest<Gift>
    {
        public Guid GiftId { get; }
        public Guid UserId { get; }

        public GetGiftQuery(Guid giftId, Guid userId)
        {
            GiftId = giftId;
            UserId = userId;
        }
    }
}
