using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prezentex.Api.Dtos;
using Prezentex.Api.Entities;
using System.ComponentModel.DataAnnotations;

namespace Prezentex.Api.Commands.Gifts
{
    public class CreateGiftCommand : IRequest<Gift>
    {
        [Required]
        public string Name { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ProductUrl { get; set; }
        public Guid UserId { get; set; }

        public CreateGiftCommand(string name, string description, decimal price, string productUrl, Guid userId)
        {
            Name = name;
            Description = description;
            Price = price;
            ProductUrl = productUrl;
            UserId = userId;
        }
    }
}
