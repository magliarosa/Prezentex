namespace Prezentex.Api.DTOs
{
    public record LoginDto(
        string Email,
        string Password);
    
    public record LoggedUserDto(
        string DisplayName,
        string Email,
        string Image,
        string UserName);

    public record RegisterDto(
        string Email,
        string Password,
        string DisplayName,
        string UserName);
}
