using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prezentex.Api.Commands.Users;
using Prezentex.Api.Dtos;
using Prezentex.Api.Entities;
using Prezentex.Api.Queries.Users;
using Prezentex.Api.Repositories;
using Prezentex.Api.Repositories.Recipients;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Prezentex.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository usersRepository;
        private readonly IGiftsRepository giftsRepository;
        private readonly IRecipientsRepository recipientsRepository;
        private readonly IMediator _mediator;

        public UsersController(IUsersRepository usersRepository, IGiftsRepository giftsRepository, IRecipientsRepository recipientsRepository, IMediator mediator)
        {
            this.usersRepository = usersRepository;
            this.giftsRepository = giftsRepository;
            this.recipientsRepository = recipientsRepository;
            _mediator = mediator;
        }

        [SwaggerOperation(Summary = "Get all users")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersAsync()
        {
            var query = new GetAllUsersQuery();
            var result = await _mediator.Send(query);
            var usersDto = result.Select(x => x.AsDto());
            return Ok(usersDto);
        }

        [SwaggerOperation(Summary = "Get user by ID")]
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDto>> GetUserAsync(Guid userId)
        {
            var query = new GetUserQuery(userId, HttpContext.GetUserId());
            var result = await _mediator.Send(query);
            return Ok(result.AsDto());
        }

        [SwaggerOperation(Summary = "Create user")]
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUserAsync(CreateUserDto userDto)
        {
            var command = new CreateUserCommand(userDto.Username, userDto.Email);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetUserAsync), new { Id = result.Id }, result.AsDto());
        }

        [SwaggerOperation(Summary = "Update user")]
        [HttpPut("{userId}")]
        public async Task<ActionResult<UserDto>> UpdateUserAsync(Guid userId, UpdateUserDto userDto)
        {
            var command = new UpdateUserCommand(
                userDto.Username,
                userDto.Email,
                userId,
                HttpContext.GetUserId());
            var result = await _mediator.Send(command);
            return Ok(result.AsDto());
        }

        [SwaggerOperation(Summary = "Delete user")]
        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUserAsync(Guid userId)
        {
            var command = new DeleteUserCommand(userId, HttpContext.GetUserId());
            await _mediator.Send(command);
            return NoContent();
        }

        [SwaggerOperation(Summary = "Add gift to user by Id")]
        [HttpPost("{userId}/gifts")]
        public async Task<ActionResult> AddGiftToUserAsync(Guid userId, AddGiftToUserDto addGiftToUserDto)
        {
            var command = new AddGiftToUserCommand(addGiftToUserDto.GiftId, userId, HttpContext.GetUserId());
            await _mediator.Send(command);
            return NoContent();
        }

        [SwaggerOperation(Summary = "Remove gift from user by Id")]
        [HttpDelete("{userId}/gifts")]
        public async Task<ActionResult> RemoveGiftFromUserAsync(Guid userId, RemoveGiftFromUserDto removeGiftFromUserDto)
        {
            var command = new RemoveGiftFromUserCommand(removeGiftFromUserDto.GiftId, userId, HttpContext.GetUserId());
            await _mediator.Send(command);
            return NoContent();
        }

        [SwaggerOperation(Summary = "Add recipient to user by Id")]
        [HttpPost("{userId}/recipients")]
        public async Task<ActionResult> AddRecipientToUserAsync(Guid userId, AddRecipientToUserDto addRecipientToUserDto)
        {
            var command = new AddRecipientToUserCommand(addRecipientToUserDto.RecipientId, userId, HttpContext.GetUserId());
            await _mediator.Send(command);
            return NoContent();
        }


        [SwaggerOperation(Summary = "Remove recipient from user by Id")]
        [HttpDelete("{userId}/recipients")]
        public async Task<ActionResult> RemoveRecipientFromUserAsync(Guid userId, RemoveRecipientFromUserDto removeRecipientFromUserDto)
        {
            var command = new RemoveRecipientFromUserCommand(removeRecipientFromUserDto.RecipientId, userId, HttpContext.GetUserId());
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
