using System.ComponentModel.DataAnnotations;

namespace Prezentex.Dtos
{
    public record GiftDto (Guid Id, DateTimeOffset CreatedDate, string Name, string Description, decimal Price, string ProductUrl);
    public record CreateGiftDto ([Required] string Name, [StringLength(500)] string Description, decimal Price, string ProductUrl);
    public record UpdateGiftDto ([Required] string Name, [StringLength(500)] string Description, decimal Price, string ProductUrl);
}