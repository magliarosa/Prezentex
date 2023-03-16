using Prezentex.Domain.Entities;
using System.Security.Claims;

namespace Api.UnitTests
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

        public ClaimsPrincipal GenerateClaimsPrincipal(Guid userId)
        {
            var claims = new List<Claim>()
            {
                new Claim("id", userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            return claimsPrincipal;
        }
    }
}
