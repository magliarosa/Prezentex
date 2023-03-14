using Prezentex.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Prezentex.Api.Dtos
{
    public record GiftDto(
        Guid Id,
        DateTimeOffset CreatedDate,
        string Name,
        string Description,
        decimal Price,
        string ProductUrl,
        IEnumerable<RecipientDto> Recipients);
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
    public record AddRecipientToGiftDto(
        [Required] Guid RecipientId);
    public record RemoveRecipientFromGiftDto(
        [Required] Guid RecipientId);

    public record RecipientDto(
        Guid Id,
        DateTimeOffset CreatedDate,
        string Name,
        string Note,
        DateTimeOffset BirthDay,
        DateTimeOffset NameDay);
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
        ICollection<GiftDto> Gifts,
        ICollection<Recipient> Recipients,
        string Email);
    public record CreateUserDto(
        [Required] string Username,
        string Email);
    public record UpdateUserDto(
        [Required] string Username,
        string Email);
    public record AddGiftToUserDto(
        [Required] Guid GiftId);
    public record RemoveGiftFromUserDto(
        [Required] Guid GiftId);
    public record AddRecipientToUserDto(
        [Required] Guid RecipientId);
    public record RemoveRecipientFromUserDto(
        [Required] Guid RecipientId);

    public record UserFacebookAuthRequestDto(
        [Required] string AccessToken);

    public record RefreshTokenRequestDto(
        string Token,
        string RefreshToken);
}