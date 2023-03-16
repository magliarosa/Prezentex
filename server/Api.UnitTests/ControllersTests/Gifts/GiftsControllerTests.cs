using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prezentex.Api.Controllers;
using Prezentex.Api.Dtos;
using Prezentex.Application.Gifts.Commands.AddRecipientToGift;
using Prezentex.Application.Gifts.Commands.CreateGift;
using Prezentex.Application.Gifts.Commands.DeleteGift;
using Prezentex.Application.Gifts.Commands.RemoveRecipientFromGift;
using Prezentex.Application.Gifts.Commands.UpdateGift;
using Prezentex.Application.Gifts.Queries.GetAllGifts;
using Prezentex.Application.Gifts.Queries.GetGift;
using Prezentex.Domain.Entities;

namespace Api.UnitTests.ControllersTests.Gifts
{
    public class GiftsControllerTests : TestBase
    {
        //naming convention: UnitOfWork_StateUnderTest_ExpectedBehavior

        private readonly Random rand = new Random();
        private readonly Mock<HttpContext> _httpContextStub = new();
        private readonly Mock<IMediator> _mediatorStub = new();

        [Fact]
        public async Task GetGiftAsync_WithUnexistingGift_ReturnsOkObjectResult()
        {
            //Arrange
            var expectedGift = CreateRandomGift();
            var query = new GetGiftQuery(
                expectedGift.Id,
                expectedGift.UserId);
            _httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(Guid.NewGuid()));
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<GetGiftQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedGift);
            var controllerContext = new ControllerContext()
            {
                HttpContext = _httpContextStub.Object
            };
            var controller = new GiftsController(_mediatorStub.Object);
            controller.ControllerContext = controllerContext;

            //Act
            var result = await controller.GetGiftAsync(Guid.NewGuid());

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetGiftsAsync_WithExistingGifts_ReturnsOkObjectResult()
        {
            //Arrange
            var expectedArray = new Gift[]
            {
                CreateRandomGift(),
                CreateRandomGift()
            };
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<GetAllGiftsQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(expectedArray);
            var controller = new GiftsController(_mediatorStub.Object);
            _httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(expectedArray.First().UserId));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = _httpContextStub.Object
            };

            //Act
            var result = await controller.GetGiftsAsync();

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task CreateGiftAsync_WithGiftToCreate_ReturnsCreatedAtActionResult()
        {
            //Arrange
            var expectedGift = CreateRandomGift();
            var giftToCreate = new CreateGiftDto(
                expectedGift.Name,
                expectedGift.Description,
                expectedGift.Price,
                expectedGift.ProductUrl);
            var controller = new GiftsController(_mediatorStub.Object);
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<CreateGiftCommand>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(expectedGift);
            _httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(Guid.NewGuid()));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = _httpContextStub.Object
            };

            //Act
            var result = await controller.CreateGiftAsync(giftToCreate);

            //Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task UpdateGiftAsync_WithExistingGift_ReturnsOkObjectResult()
        {
            //Arrange
            var existingGift = CreateRandomGift();
            var giftToUpdate = new UpdateGiftDto(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                rand.Next(1000),
                Guid.NewGuid().ToString());
            var expectedGift = new Gift
            {
                Id = existingGift.Id,
                CreatedDate = existingGift.CreatedDate,
                UpdatedDate = existingGift.UpdatedDate,
                Description = giftToUpdate.Description,
                Name = giftToUpdate.Name,
                Price = giftToUpdate.Price,
                ProductUrl = giftToUpdate.ProductUrl,
                UserId = existingGift.UserId,
            };
            var giftId = existingGift.Id;
            var controller = new GiftsController(_mediatorStub.Object);
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<UpdateGiftCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedGift);
            _httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(existingGift.UserId));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = _httpContextStub.Object
            };

            //Act
            var result = await controller.UpdateGiftAsync(giftId, giftToUpdate);

            //Assert
            var updatedGift = (result.Result as OkObjectResult).Value as GiftDto;
            updatedGift.Should().BeEquivalentTo(
                expectedGift,
                options => 
                options.ExcludingMissingMembers());
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteGiftAsync_WithExistingGift_ReturnsNoContentResult()
        {
            //Arrange
            var existingGift = CreateRandomGift();
            var giftId = existingGift.Id;
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<DeleteGiftCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);
            var controller = new GiftsController(_mediatorStub.Object);
            _httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(existingGift.UserId));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = _httpContextStub.Object
            };

            //Act
            var result = await controller.DeleteGiftAsync(giftId);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task AddRecipientToGiftAsync_WithExistingGiftAndRecipient_ReturnsNoContent()
        {
            //Arrange
            var existingGift = CreateRandomGift();
            var giftId = existingGift.Id;
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<AddRecipientToGiftCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Unit.Value);
            var existingRecipient = CreateRandomRecipient();
            existingRecipient.UserId = existingGift.UserId;
            var recipientId = existingRecipient.Id;
            
            var controller = new GiftsController(_mediatorStub.Object);
            var addRecipientDto = new AddRecipientToGiftDto(recipientId);
            _httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(existingGift.UserId));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = _httpContextStub.Object
            };

            //Act
            var result = await controller.AddRecipientToGiftAsync(giftId, addRecipientDto);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task RemoveRecipientFromGiftAsync_WithExistingGiftAndRecipient_ReturnsNoContent()
        {
            //Arrange
            var existingGift = CreateRandomGift();
            var giftId = existingGift.Id;
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<RemoveRecipientFromGiftCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);
            var existingRecipient = CreateRandomRecipient();
            existingRecipient.UserId = existingGift.UserId;
            var recipientId = existingRecipient.Id;
            existingGift.Recipients.Add(existingRecipient);
            var controller = new GiftsController(_mediatorStub.Object);
            _httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(existingGift.UserId));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = _httpContextStub.Object
            };
            var removeRecipientDto = new RemoveRecipientFromGiftDto(recipientId);

            //Act
            var result = await controller.RemoveRecipientFromGiftAsync(giftId, removeRecipientDto);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}