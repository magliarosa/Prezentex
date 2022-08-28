﻿namespace Prezentex.Api.Entities
{
    public record Gift : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ProductUrl { get; set; }
    }
}