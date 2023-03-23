using Prezentex.Domain.Entities;

namespace Application.UnitTests
{
    public class TestBase
    {
        private readonly Random rand = new Random();
        protected readonly CancellationToken _cancellationToken = new CancellationToken();


        public User CreateRandomUser()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                UserName = Guid.NewGuid().ToString()
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
                UpdatedDate = DateTimeOffset.UtcNow,
                UserId = Guid.NewGuid()
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
