using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prezentex.Api.Dtos;
using Prezentex.Application.Gifts.Commands.AddRecipientToGift;
using Prezentex.Application.Gifts.Commands.CreateGift;
using Prezentex.Application.Gifts.Commands.DeleteGift;
using Prezentex.Application.Gifts.Commands.RemoveRecipientFromGift;
using Prezentex.Application.Gifts.Commands.UpdateGift;
using Prezentex.Application.Gifts.Queries.GetAllGifts;
using Prezentex.Application.Gifts.Queries.GetGift;
using Swashbuckle.AspNetCore.Annotations;

namespace Prezentex.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class GiftsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GiftsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SwaggerOperation(Summary = "Get all gifts")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GiftDto>>> GetGiftsAsync()
        {
            var query = new GetAllGiftsQuery();
            var result = await _mediator.Send(query);
            var giftsDto = result.Select(x => x.AsDto());
            return Ok(giftsDto);
        }

        [SwaggerOperation(Summary = "Get gift by ID")]
        [HttpGet("{id}")]
        public async Task<ActionResult<GiftDto>> GetGiftAsync(Guid id)
        {
            var query = new GetGiftQuery(id, HttpContext.GetUserId());
            var result = await _mediator.Send(query);
            var giftDto = result.AsDto();
            return Ok(giftDto);
        }

        [SwaggerOperation(Summary = "Create gift")]
        [HttpPost]
        public async Task<ActionResult<GiftDto>> CreateGiftAsync(CreateGiftDto giftDto)
        {
            var command = new CreateGiftCommand(giftDto.Name, giftDto.Description, giftDto.Price, giftDto.ProductUrl, HttpContext.GetUserId());
            var result = await _mediator.Send(command);
            var newGiftDto = result.AsDto();
            return CreatedAtAction(nameof(GetGiftAsync), new { ID = newGiftDto.Id }, result);
        }

        [SwaggerOperation(Summary = "Update gift")]
        [HttpPut("{giftId}")]
        public async Task<ActionResult<GiftDto>> UpdateGiftAsync(Guid giftId, UpdateGiftDto giftDto)
        {
            var command = new UpdateGiftCommand(giftId, giftDto.Name, giftDto.Description, giftDto.Price, giftDto.ProductUrl, HttpContext.GetUserId());
            var result = await _mediator.Send(command);
            var updatedGiftDto = result.AsDto();
            return Ok(updatedGiftDto);
        }

        [SwaggerOperation(Summary = "Delete gift")]
        [HttpDelete("{giftId}")]
        public async Task<ActionResult> DeleteGiftAsync(Guid giftId)
        {
            var command = new DeleteGiftCommand(giftId, HttpContext.GetUserId());
            await _mediator.Send(command);
            return NoContent();
        }

        [SwaggerOperation(Summary = "Add recipient to gift by Id")]
        [HttpPost("{giftId}/recipients")]
        public async Task<ActionResult> AddRecipientToGiftAsync(Guid giftId, AddRecipientToGiftDto addRecipientToGiftDto)
        {
            var command = new AddRecipientToGiftCommand(addRecipientToGiftDto.RecipientId, giftId, HttpContext.GetUserId());
            await _mediator.Send(command);
            return NoContent();
        }


        [SwaggerOperation(Summary = "Remove recipient from gift by Id")]
        [HttpDelete("{giftId}/recipients")]
        public async Task<ActionResult> RemoveRecipientFromGiftAsync(Guid giftId, RemoveRecipientFromGiftDto removeRecipientFromGiftDto)
        {
            var command = new RemoveRecipientFromGiftCommand(removeRecipientFromGiftDto.RecipientId, giftId, HttpContext.GetUserId());
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
