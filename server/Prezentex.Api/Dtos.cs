using Prezentex.Api.Entities;
using System.ComponentModel.DataAnnotations;

namespace Prezentex.Api.Dtos
{
    public record GiftDto(
        Guid Id,
        DateTimeOffset CreatedDate,
        string Name,
        string Description,
        decimal Price,
        string ProductUrl);
    public record CreateGiftDto(
        [Required] string Name,
        [StringLength(500)] string Description,
        decimal Price,
        string ProductUrl);
    public record UpdateGiftDto(
        [Required] string Name,
        [StringLength(500)] string Description,
        decimal Price,
        string ProductUrl);

    public record RecipientDto(
        Guid Id,
        DateTimeOffset CreatedDate,
        string Name,
        string Note,
        DateTimeOffset BirthDay,
        DateTimeOffset NameDay,
        IEnumerable<Gift> Gifts);
    public record CreateRecipientDto(
        [Required] string Name,
        string Note,
        DateTimeOffset BirthDay,
        DateTimeOffset NameDay);
    public record UpdateRecipientDto(
        [Required] string Name,
        string Note,
        DateTimeOffset BirthDay,
        DateTimeOffset NameDay);

    public record UserDto(
        Guid Id,
        DateTimeOffset CreatedDate,
        string Username,
        ICollection<Gift> Gifts,
        ICollection<Recipient> Recipients);
    public record CreateUserDto(
        [Required] string Username);
    public record UpdateUserDto(
        [Required] string Username);
    public record AddGiftToUserDto(
        [Required] Guid GiftId);
    public record RemoveGiftFromUserDto(
        [Required] Guid GiftId);
    public record AddRecipientToUserDto(
        [Required] Guid RecipientId);
    public record RemoveRecipientFromUserDto(
        [Required] Guid RecipientId);
}