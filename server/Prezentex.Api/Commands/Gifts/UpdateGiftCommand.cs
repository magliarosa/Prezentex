using MediatR;
using Prezentex.Api.Entities;
using System.ComponentModel.DataAnnotations;

namespace Prezentex.Api.Commands.Gifts
{
    public class UpdateGiftCommand : IRequest<Gift>
    {
        [Required]
        public Guid GiftId { get; set; }
        [Required]
        public string Name { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ProductUrl { get; set; }
        public Guid UserId { get; set; }

        public UpdateGiftCommand(Guid giftId, string name, string description, decimal price, string productUrl, Guid userId)
        {
            Name = name;
            Description = description;
            Price = price;
            ProductUrl = productUrl;
            UserId = userId;
            GiftId = giftId;
        }
    }
}
