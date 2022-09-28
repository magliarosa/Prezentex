using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prezentex.Api.Dtos;
using Prezentex.Api.Entities;
using Prezentex.Api.Repositories;
using Prezentex.Api.Repositories.Recipients;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Prezentex.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository usersRepository;
        private readonly IGiftsRepository giftsRepository;
        private readonly IRecipientsRepository recipientsRepository;

        public UsersController(IUsersRepository usersRepository, IGiftsRepository giftsRepository, IRecipientsRepository recipientsRepository)
        {
            this.usersRepository = usersRepository;
            this.giftsRepository = giftsRepository;
            this.recipientsRepository = recipientsRepository;
        }

        [SwaggerOperation(Summary = "Get all users")]
        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var users = await usersRepository.GetUsersAsync();
            var usersDto = users.Select(user => user.AsDto());
            return usersDto;
        }

        [SwaggerOperation(Summary = "Get user by ID")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserAsync(Guid id)
        {
            var user = await usersRepository.GetUserAsync(id);

            if (user == null)
                return NotFound();

            return user.AsDto();
        }

        [SwaggerOperation(Summary = "Create user")]
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUserAsync(CreateUserDto userDto)
        {
            var newUser = new User
            {
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow,
                Id = Guid.NewGuid(),
                Username = userDto.Username,
            };

            await usersRepository.CreateUserAsync(newUser);

            return CreatedAtAction(nameof(GetUserAsync), new { Id = newUser.Id }, newUser.AsDto());
        }

        [SwaggerOperation(Summary = "Update user")]
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUserAsync(Guid id, UpdateUserDto userDto)
        {
            var existingUser = await usersRepository.GetUserAsync(id);

            if (existingUser == null)
                return NotFound();

            var updatedUser = new User
            {
                Id = existingUser.Id,
                Username = userDto.Username,
                UpdatedDate = DateTimeOffset.UtcNow,
                CreatedDate = existingUser.CreatedDate,
                Gifts = existingUser.Gifts,
                Recipients = existingUser.Recipients,
            };

            await usersRepository.UpdateUserAsync(updatedUser);

            return Ok(updatedUser.AsDto());
        }

        [SwaggerOperation(Summary = "Delete user")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUserAsync(Guid id)
        {
            var existingUser = await usersRepository.GetUserAsync(id);
            if (existingUser == null)
                return NotFound();

            await usersRepository.DeleteUserAsync(id);

            return NoContent();
        }

        [SwaggerOperation(Summary = "Add gift to user by Id")]
        [HttpPost("{userId}/gifts")]
        public async Task<ActionResult> AddGiftToUserAsync(Guid userId, AddGiftToUserDto addGiftToUserDto)
        {
            var giftId = addGiftToUserDto.GiftId;
            var user = await usersRepository.GetUserAsync(userId);
            var gift = await giftsRepository.GetGiftAsync(addGiftToUserDto.GiftId);
            
            if (user == null || gift == null)
                return NotFound();
            
            await usersRepository.AddGiftToUserAsync(userId, giftId);

            return NoContent();
        }


        [SwaggerOperation(Summary = "Remove gift from user by Id")]
        [HttpDelete("{userId}/gifts")]
        public async Task<ActionResult> RemoveGiftFromUserAsync(Guid userId, RemoveGiftFromUserDto removeGiftFromUserDto)
        {
            var user = await usersRepository.GetUserAsync(userId);
            var giftId = removeGiftFromUserDto.GiftId;
            var gift = await giftsRepository.GetGiftAsync(giftId);

            if (user == null || gift == null || !user.Gifts.Any(gift => gift.Id == giftId))
                return NotFound();
            
            await usersRepository.RemoveGiftFromUserAsync(userId, giftId);

            return NoContent();
        }

        [SwaggerOperation(Summary = "Add recipient to user by Id")]
        [HttpPost("{userId}/recipients")]
        public async Task<ActionResult> AddRecipientToUserAsync(Guid userId, AddRecipientToUserDto addRecipientToUserDto)
        {
            var recipientId = addRecipientToUserDto.RecipientId;
            var user = await usersRepository.GetUserAsync(userId);
            var recipient = await recipientsRepository.GetRecipientAsync(recipientId);

            if (user == null || recipient == null)
                return NotFound();

            await usersRepository.AddRecipientToUserAsync(userId, recipientId);

            return NoContent();
        }


        [SwaggerOperation(Summary = "Remove recipient from user by Id")]
        [HttpDelete("{userId}/recipients")]
        public async Task<ActionResult> RemoveRecipientFromUserAsync(Guid userId, RemoveRecipientFromUserDto removeRecipientFromUserDto)
        {
            var user = await usersRepository.GetUserAsync(userId);
            var recipientId = removeRecipientFromUserDto.RecipientId;
            var recipient = await recipientsRepository.GetRecipientAsync(recipientId);

            if (user == null || recipient == null || !user.Recipients.Any(recipient => recipient.Id == recipientId))
                return NotFound();

            await usersRepository.RemoveRecipientFromUserAsync(userId, recipientId);

            return NoContent();
        }
    }
}
