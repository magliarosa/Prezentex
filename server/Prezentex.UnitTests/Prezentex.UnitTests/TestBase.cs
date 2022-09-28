using Prezentex.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prezentex.UnitTests
{
    public class TestBase
    {
        private readonly Random rand = new Random();

        public User CreateRandomUser()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow,
                Username = Guid.NewGuid().ToString()
            };
        }

        public Gift CreateRandomGift()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Price = rand.Next(1000),
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow
            };

        }

        public Recipient CreateRandomRecipient()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow,
                BirthDay = DateTimeOffset.UtcNow.Date,
                NameDay = DateTimeOffset.UtcNow.Date,
                Note = Guid.NewGuid().ToString(),
            };
        }
    }
}
