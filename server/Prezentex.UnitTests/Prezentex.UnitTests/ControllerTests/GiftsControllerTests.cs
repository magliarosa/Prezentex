using Moq;
using Xunit;
using Prezentex.Api.Repositories;
using System;
using Prezentex.Api.Entities;
using Prezentex.Api.Controllers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Prezentex.Api.Dtos;
using FluentAssertions;
using Prezentex.Api.Repositories.Recipients;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;

namespace Prezentex.UnitTests.ControllerTests
{
    public class GiftsControllerTests : TestBase
    {
        //naming convention: UnitOfWork_StateUnderTest_ExpectedBehavior

        private readonly Mock<IGiftsRepository> giftsRepositoryStub = new();
        private readonly Mock<IRecipientsRepository> recipientsRepositoryStub = new();
        private readonly Random rand = new Random();
        private readonly Mock<HttpContext> httpContextStub = new();

        [Fact]
        public async Task GetGiftAsync_WithUnexistingGift_ReturnsNotFound()
        {
            //Arrange
            giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Gift)null);
            httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(Guid.NewGuid()));
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContextStub.Object
            };
            var controller = new GiftsController(giftsRepositoryStub.Object, recipientsRepositoryStub.Object);
            controller.ControllerContext = controllerContext;
            //Act
            var result = await controller.GetGiftAsync(Guid.NewGuid());

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetGiftAsync_WithExistingGift_ReturnsExpectedGift()
        {
            //Arrange
            var expectedGift = CreateRandomGift();
            httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(expectedGift.UserId));
            giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedGift);
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContextStub.Object
            };
            var controller = new GiftsController(giftsRepositoryStub.Object, recipientsRepositoryStub.Object);
            controller.ControllerContext = controllerContext;
            //Act
            var result = await controller.GetGiftAsync(Guid.NewGuid());

            //Assert
            result.Value.Should().BeEquivalentTo(
                expectedGift,
                options => options
                    .ExcludingMissingMembers());
        }

        [Fact]
        public async Task GetGiftsAsync_WithUnexistingGifts_ReturnsEmptyArray()
        {
            //Arrange
            giftsRepositoryStub.Setup(repo => repo.GetGiftsAsync())
                .ReturnsAsync(new Gift[0]);
            var controller = new GiftsController(giftsRepositoryStub.Object, recipientsRepositoryStub.Object);
            
            //Act
            var result = await controller.GetGiftsAsync();

            //Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetGiftsAsync_WithExistingGifts_ReturnsArrayWithGifts()
        {
            //Arrange
            var expectedArray = new Gift[]
            {
                CreateRandomGift(),
                CreateRandomGift()
            };
            giftsRepositoryStub.Setup(repo => repo.GetGiftsAsync())
                .ReturnsAsync(expectedArray);
            var controller = new GiftsController(giftsRepositoryStub.Object, recipientsRepositoryStub.Object);
            httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(expectedArray.First().UserId));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextStub.Object
            };

            //Act
            var result = await controller.GetGiftsAsync();

            //Assert
            result.Should().BeEquivalentTo(
                expectedArray,
                options => options
                    .ExcludingMissingMembers());
        }

        [Fact]
        public async Task CreateGiftAsync_WithGiftToCreate_ReturnsCreatedGift()
        {
            //Arrange
            var giftToCreate = new CreateGiftDto(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                rand.Next(2000),
                Guid.NewGuid().ToString());
            var controller = new GiftsController(giftsRepositoryStub.Object, recipientsRepositoryStub.Object);
            httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(Guid.NewGuid()));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextStub.Object
            };

            //Act
            var result = await controller.CreateGiftAsync(giftToCreate);

            //Assert
            var createdGift = (result.Result as CreatedAtActionResult).Value as GiftDto;
            giftToCreate.Should().BeEquivalentTo(
                createdGift,
                options => options
                .ComparingByMembers<GiftDto>()
                .ExcludingMissingMembers());
            createdGift.Id.Should().NotBeEmpty();
            createdGift.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, new TimeSpan(0, 0, 1));
        }

        [Fact]
        public async Task UpdateGiftAsync_WithExistingGift_ReturnsModifiedGift()
        {
            //Arrange
            var existingGift = CreateRandomGift();
            var giftToUpdate = new UpdateGiftDto(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                rand.Next(1000),
                Guid.NewGuid().ToString());
            var giftId = existingGift.Id;
            giftsRepositoryStub.Setup(options => options.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingGift);
            var controller = new GiftsController(giftsRepositoryStub.Object, recipientsRepositoryStub.Object);
            httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(existingGift.UserId));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextStub.Object
            };

            //Act
            var result = await controller.UpdateGiftAsync(giftId, giftToUpdate);

            //Assert
            var updatedGift = (result.Result as OkObjectResult).Value as GiftDto;
            updatedGift.Should().BeEquivalentTo(giftToUpdate);
        }

        [Fact]
        public async Task UpdateGiftAsync_WithUnexistingGift_ReturnsNotFound()
        {
            //Arrange
            var giftToUpdate = new UpdateGiftDto(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                rand.Next(1000),
                Guid.NewGuid().ToString());
            var giftId = Guid.NewGuid();
            giftsRepositoryStub.Setup(options => options.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Gift)null);
            var controller = new GiftsController(giftsRepositoryStub.Object, recipientsRepositoryStub.Object);

            //Act
            var result = await controller.UpdateGiftAsync(giftId, giftToUpdate);

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteGiftAsync_WithExistingGift_ReturnsNoContent()
        {
            //Arrange
            var existingGift = CreateRandomGift();
            var giftId = existingGift.Id;
            giftsRepositoryStub.Setup(options => options.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingGift);
            var controller = new GiftsController(giftsRepositoryStub.Object, recipientsRepositoryStub.Object);
            httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(existingGift.UserId));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextStub.Object
            };

            //Act
            var result = await controller.DeleteGiftAsync(giftId);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteGiftAsync_WithUnexistingGift_ReturnsNotFound()
        {
            //Arrange
            var giftId = Guid.NewGuid();
            giftsRepositoryStub.Setup(options => options.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Gift)null);
            var controller = new GiftsController(giftsRepositoryStub.Object, recipientsRepositoryStub.Object);

            //Act
            var result = await controller.DeleteGiftAsync(giftId);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AddRecipientToGiftAsync_WithExistingGiftAndRecipient_ReturnsNoContent()
        {
            //Arrange
            var existingGift = CreateRandomGift();
            var giftId = existingGift.Id;
            giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingGift);
            var existingRecipient = CreateRandomRecipient();
            existingRecipient.UserId = existingGift.UserId;
            var recipientId = existingRecipient.Id;
            recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingRecipient);
            var controller = new GiftsController(
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);
            var addRecipientDto = new AddRecipientToGiftDto(recipientId);
            httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(existingGift.UserId));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextStub.Object
            };

            //Act
            var result = await controller.AddRecipientToGiftAsync(giftId, addRecipientDto);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task AddRecipientToGiftAsync_WithUnexistingRecipient_ReturnsNotFound()
        {
            //Arrange
            var existingGift = CreateRandomGift();
            var giftId = existingGift.Id;
            giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingGift);
            var recipientId = Guid.NewGuid();
            recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Recipient)null);
            var controller = new GiftsController(
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);


            var addRecipientDto = new AddRecipientToGiftDto(recipientId);

            //Act
            var result = await controller.AddRecipientToGiftAsync(giftId, addRecipientDto);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AddRecipientToGiftAsync_WithUnexistingGift_ReturnsNotFound()
        {
            //Arrange
            var giftId = Guid.NewGuid();
            giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Gift)null);
            var existingRecipient = CreateRandomRecipient();
            var recipientId = existingRecipient.Id;
            recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingRecipient);
            var controller = new GiftsController(
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);
            var addRecipientDto = new AddRecipientToGiftDto(recipientId);

            //Act
            var result = await controller.AddRecipientToGiftAsync(giftId, addRecipientDto);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task RemoveRecipientFromGiftAsync_WithUnexistingGiftAndRecipient_ReturnsNotFound()
        {
            //Arrange
            var existingGift = CreateRandomGift();
            var giftId = existingGift.Id;
            giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingGift);
            var recipientId = Guid.NewGuid();
            recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Recipient)null);
            var controller = new GiftsController(
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);
            var removeRecipientDto = new RemoveRecipientFromGiftDto(recipientId);

            //Act
            var result = await controller.RemoveRecipientFromGiftAsync(giftId, removeRecipientDto);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task RemoveRecipientFromGiftAsync_WithExistingGiftAndRecipient_ReturnsNoContent()
        {
            //Arrange
            var existingGift = CreateRandomGift();
            var giftId = existingGift.Id;
            giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingGift);
            var existingRecipient = CreateRandomRecipient();
            existingRecipient.UserId = existingGift.UserId;
            var recipientId = existingRecipient.Id;
            recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingRecipient);
            existingGift.Recipients.Add(existingRecipient);
            var controller = new GiftsController(
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);
            httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(existingGift.UserId));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextStub.Object
            };
            var removeRecipientDto = new RemoveRecipientFromGiftDto(recipientId);

            //Act
            var result = await controller.RemoveRecipientFromGiftAsync(giftId, removeRecipientDto);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task RemoveRecipientFromGiftAsync_WithUnexistingGift_ReturnsNotFound()
        {
            //Arrange
            var giftId = Guid.NewGuid();
            giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Gift)null);
            var existingRecipient = CreateRandomRecipient();
            var recipientId = existingRecipient.Id;
            recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingRecipient);
            var controller = new GiftsController(
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);

            var removeRecipientDto = new RemoveRecipientFromGiftDto(recipientId);

            //Act
            var result = await controller.RemoveRecipientFromGiftAsync(giftId, removeRecipientDto);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}