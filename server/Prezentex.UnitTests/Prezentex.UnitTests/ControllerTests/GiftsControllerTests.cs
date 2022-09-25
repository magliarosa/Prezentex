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

namespace Prezentex.UnitTests.ControllerTests
{
    public class GiftsControllerTests
    {
        //naming convention: UnitOfWork_StateUnderTest_ExpectedBehavior

        private readonly Mock<IGiftsRepository> repositoryStub = new();
        private readonly Random rand = new Random();

        [Fact]
        public async Task GetGiftAsync_WithUnexistingGift_ReturnsNotFound()
        {
            //Arrange
            repositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Gift)null);

            var controller = new GiftsController(repositoryStub.Object);

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
            repositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedGift);
            var controller = new GiftsController(repositoryStub.Object);

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
            repositoryStub.Setup(repo => repo.GetGiftsAsync())
                .ReturnsAsync(new Gift[0]);
            var controller = new GiftsController(repositoryStub.Object);

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
            repositoryStub.Setup(repo => repo.GetGiftsAsync())
                .ReturnsAsync(expectedArray);
            var controller = new GiftsController(repositoryStub.Object);

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
            var controller = new GiftsController(repositoryStub.Object);

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
            createdGift.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, new TimeSpan(0,0,1));
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
            repositoryStub.Setup(options => options.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingGift);
            var controller = new GiftsController(repositoryStub.Object);

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
            repositoryStub.Setup(options => options.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Gift)null);
            var controller = new GiftsController(repositoryStub.Object);

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
            repositoryStub.Setup(options => options.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingGift);
            var controller = new GiftsController(repositoryStub.Object);

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
            repositoryStub.Setup(options => options.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Gift)null);
            var controller = new GiftsController(repositoryStub.Object);

            //Act
            var result = await controller.DeleteGiftAsync(giftId);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        private Gift CreateRandomGift()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Price = rand.Next(1000),
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}