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
                options =>
                {
                    options
                    .ComparingByMembers<Gift>()
                    .ExcludingMissingMembers();
                    return options;
                });
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