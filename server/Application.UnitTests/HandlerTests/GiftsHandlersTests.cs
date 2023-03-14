using FluentAssertions;
using MediatR;
using Moq;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Application.Exceptions;
using Prezentex.Application.Gifts.Commands.AddRecipientToGift;
using Prezentex.Application.Gifts.Commands.CreateGift;
using Prezentex.Application.Gifts.Commands.DeleteGift;
using Prezentex.Application.Gifts.Commands.RemoveRecipientFromGift;
using Prezentex.Application.Gifts.Commands.UpdateGift;
using Prezentex.Application.Gifts.Queries.GetAllGifts;
using Prezentex.Application.Gifts.Queries.GetGift;
using Prezentex.Domain.Entities;

namespace Prezentex.UnitTests.HandlerTests
{
    public class GiftsHandlersTests : TestBase
    {
        //naming convention: UnitOfWork_StateUnderTest_ExpectedBehavior

        private readonly Mock<IGiftsRepository> _giftsRepositoryStub = new();
        private readonly Mock<IRecipientsRepository> _recipientsRepositoryStub = new();
        private readonly Random rand = new Random();

        [Fact]
        public async Task GetGift_WithUnexistingGift_ThrowsResourceNotFoundException()
        {
            //Arrange
            _giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Gift)null);
            var querry = new GetGiftQuery(
                Guid.NewGuid(),
                Guid.NewGuid());
            var handler = new GetGiftHandler(_giftsRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(querry, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task GetGift_WithExistingGift_ReturnsExpectedGift()
        {
            //Arrange
            var expectedGift = CreateRandomGift();
            _giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedGift);
            var querry = new GetGiftQuery(
                expectedGift.Id,
                expectedGift.UserId);
            var handler = new GetGiftHandler(_giftsRepositoryStub.Object);
            //Act
            var result = await handler.Handle(querry, _cancellationToken);

            //Assert
            result.Should().BeEquivalentTo(
                expectedGift,
                options => options
                    .ExcludingMissingMembers());
        }

        [Fact]
        public async Task GetAllGifts_WithUnexistingGifts_ReturnsEmptyArray()
        {
            //Arrange
            _giftsRepositoryStub.Setup(repo => repo.GetGiftsAsync())
                .ReturnsAsync(new Gift[0]);
            var handler = new GetAllGiftsHandler(_giftsRepositoryStub.Object);
            var query = new GetAllGiftsQuery();

            //Act
            var result = await handler.Handle(query, _cancellationToken);

            //Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllGifts_WithExistingGifts_ReturnsArrayWithGifts()
        {
            //Arrange
            var expectedArray = new Gift[]
            {
                CreateRandomGift(),
                CreateRandomGift()
            };
            _giftsRepositoryStub.Setup(repo => repo.GetGiftsAsync())
                .ReturnsAsync(expectedArray);
            var handler = new GetAllGiftsHandler(_giftsRepositoryStub.Object);
            var query = new GetAllGiftsQuery();

            //Act
            var result = await handler.Handle(query, _cancellationToken);

            //Assert
            result.Should().BeEquivalentTo(
                expectedArray,
                options => options
                    .ExcludingMissingMembers());
        }

        [Fact]
        public async Task CreateGift_WithGiftToCreate_ReturnsCreatedGift()
        {
            //Arrange
            var command = new CreateGiftCommand(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                rand.Next(2000),
                Guid.NewGuid().ToString(),
                Guid.NewGuid());
            var handler = new CreateGiftHandler(_giftsRepositoryStub.Object);

            //Act
            var result = await handler.Handle(command, _cancellationToken);

            //Assert
            var createdGift = result;
            command.Should().BeEquivalentTo(
                createdGift,
                options => options
                .ComparingByMembers<Gift>()
                .ExcludingMissingMembers());
            createdGift.Id.Should().NotBeEmpty();
            createdGift.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, new TimeSpan(0, 0, 1));
        }

        [Fact]
        public async Task UpdateGift_WithExistingGift_ReturnsModifiedGift()
        {
            //Arrange
            var existingGift = CreateRandomGift();
            _giftsRepositoryStub.Setup(options => options.GetGiftAsync(It.IsAny<Guid>()))
           .ReturnsAsync(existingGift);
            var command = new UpdateGiftCommand(
                existingGift.Id,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                rand.Next(1000),
                Guid.NewGuid().ToString(),
                existingGift.UserId);
            var giftId = existingGift.Id;
            var handler = new UpdateGiftHandler(_giftsRepositoryStub.Object);

            //Act
            var result = await handler.Handle(command, _cancellationToken);
            //Assert
            var updatedGift = result;
            updatedGift.Should().BeEquivalentTo(
                command,
                options => options
                .ComparingByMembers<Gift>()
                .ExcludingMissingMembers());
        }

        [Fact]
        public async Task UpdateGift_WithUnexistingGift_ThrowsResourceNotfoundException()
        {
            //Arrange
            var giftId = Guid.NewGuid();
            _giftsRepositoryStub.Setup(options => options.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Gift)null);
            var command = new UpdateGiftCommand(
                giftId,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                rand.Next(1000),
                Guid.NewGuid().ToString(),
                Guid.NewGuid());
            var handler = new UpdateGiftHandler(_giftsRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task DeleteGift_WithExistingGift_ReturnsUnit()
        {
            //Arrange
            var existingGift = CreateRandomGift();
            var command = new DeleteGiftCommand(
                existingGift.Id,
                existingGift.UserId
                );
            _giftsRepositoryStub.Setup(options => options.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingGift);
            var handler = new DeleteGiftHandler(_giftsRepositoryStub.Object);

            //Act
            var result = await handler.Handle(command, _cancellationToken);

            //Assert
            result.Should().BeOfType<Unit>();
        }

        [Fact]
        public async Task DeleteGift_WithUnexistingGift_ThrowsResourceNotFoundException()
        {
            //Arrange
            var giftId = Guid.NewGuid();
            _giftsRepositoryStub.Setup(options => options.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Gift)null);
            var command = new DeleteGiftCommand(
                Guid.NewGuid(),
                Guid.NewGuid());
            var handler = new DeleteGiftHandler(_giftsRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task AddRecipientToGift_WithExistingGiftAndRecipient_ReturnsUnit()
        {
            //Arrange
            var existingGift = CreateRandomGift();
            var giftId = existingGift.Id;
            _giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingGift);
            var existingRecipient = CreateRandomRecipient();
            existingRecipient.UserId = existingGift.UserId;
            var recipientId = existingRecipient.Id;
            _recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingRecipient);
            var command = new AddRecipientToGiftCommand(
                recipientId,
                giftId,
                existingGift.UserId);
            var handler = new AddRecipientToGiftHandler(_giftsRepositoryStub.Object, _recipientsRepositoryStub.Object);

            //Act
            var result = await handler.Handle(command, _cancellationToken);

            //Assert
            result.Should().BeOfType<Unit>();
        }

        [Fact]
        public async Task AddRecipientToGift_WithUnexistingRecipient_ThrowsResourceNotFoundException()
        {
            //Arrange
            var existingGift = CreateRandomGift();
            var giftId = existingGift.Id;
            _giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingGift);
            var recipientId = Guid.NewGuid();
            _recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Recipient)null);
            var command = new AddRecipientToGiftCommand(
                recipientId,
                giftId,
                existingGift.UserId);
            var handler = new AddRecipientToGiftHandler(_giftsRepositoryStub.Object, _recipientsRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task AddRecipientToGift_WithUnexistingGift_ThrowsResourceNotFoundException()
        {
            //Arrange
            var giftId = Guid.NewGuid();
            _giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Gift)null);
            var existingRecipient = CreateRandomRecipient();
            var recipientId = existingRecipient.Id;
            _recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingRecipient);
            var command = new AddRecipientToGiftCommand(
                recipientId,
                giftId,
                existingRecipient.UserId);
            var handler = new AddRecipientToGiftHandler(_giftsRepositoryStub.Object, _recipientsRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task RemoveRecipientFromGift_WithUnexistingGiftAndRecipient_ThrowsResourceNotFoundException()
        {
            //Arrange
            var existingGift = CreateRandomGift();
            var giftId = existingGift.Id;
            _giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingGift);
            var recipientId = Guid.NewGuid();
            _recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Recipient)null);
            var command = new RemoveRecipientFromGiftCommand(
                recipientId,
                giftId,
                existingGift.UserId);
            var handler = new RemoveRecipientFromGiftHandler(_giftsRepositoryStub.Object, _recipientsRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task RemoveRecipientFromGift_WithExistingGiftAndRecipient_ReturnsUnit()
        {
            //Arrange
            var existingGift = CreateRandomGift();
            var giftId = existingGift.Id;
            _giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingGift);
            var existingRecipient = CreateRandomRecipient();
            existingRecipient.UserId = existingGift.UserId;
            var recipientId = existingRecipient.Id;
            _recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingRecipient);
            existingGift.Recipients.Add(existingRecipient);
            var command = new RemoveRecipientFromGiftCommand(
                 recipientId,
                 giftId,
                 existingGift.UserId);
            var handler = new RemoveRecipientFromGiftHandler(_giftsRepositoryStub.Object, _recipientsRepositoryStub.Object);

            //Act
            var result = await handler.Handle(command, _cancellationToken);

            //Assert
            result.Should().BeOfType<Unit>();
        }

        [Fact]
        public async Task RemoveRecipientFromGift_WithUnexistingGift_ThrowsResourceNotFoundException()
        {
            //Arrange
            var giftId = Guid.NewGuid();
            _giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Gift)null);
            var existingRecipient = CreateRandomRecipient();
            var recipientId = existingRecipient.Id;
            _recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingRecipient);
            var command = new RemoveRecipientFromGiftCommand(
                recipientId,
                giftId,
                existingRecipient.UserId);
            var handler = new RemoveRecipientFromGiftHandler(_giftsRepositoryStub.Object, _recipientsRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }
    }
}