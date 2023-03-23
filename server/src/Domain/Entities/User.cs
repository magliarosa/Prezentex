using Microsoft.AspNetCore.Identity;

namespace Prezentex.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string DisplayName { get; set; }
        public ICollection<Gift> Gifts { get; set; } = new List<Gift>();
        public ICollection<Recipient> Recipients { get; set; } = new List<Recipient>();
    }
}
