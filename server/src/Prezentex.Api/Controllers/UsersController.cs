using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prezentex.Api.Dtos;
using Prezentex.Application.Users.Commands.AddGiftToUser;
using Prezentex.Application.Users.Commands.AddRecipientToUser;
using Prezentex.Application.Users.Commands.CreateUser;
using Prezentex.Application.Users.Commands.DeleteUser;
using Prezentex.Application.Users.Commands.RemoveGiftFromUser;
using Prezentex.Application.Users.Commands.RemoveRecipientFromUser;
using Prezentex.Application.Users.Commands.UpdateUser;
using Prezentex.Application.Users.Queries.GetAllUsers;
using Prezentex.Application.Users.Queries.GetUser;
using Swashbuckle.AspNetCore.Annotations;

namespace Prezentex.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
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
            var command = new CreateUserCommand(userDto.UserName, userDto.Email);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetUserAsync), new { Id = result.Id }, result.AsDto());
        }

        [SwaggerOperation(Summary = "Update user")]
        [HttpPut("{userId}")]
        public async Task<ActionResult<UserDto>> UpdateUserAsync(Guid userId, UpdateUserDto userDto)
        {
            var command = new UpdateUserCommand(
                userDto.UserName,
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
