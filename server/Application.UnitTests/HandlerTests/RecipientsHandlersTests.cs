using Application.UnitTests;
using FluentAssertions;
using MediatR;
using Moq;
using Prexentex.Application.Recipients.Commands.UpdateRecipient;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Application.Exceptions;
using Prezentex.Application.Recipients.Commands.CreateRecipient;
using Prezentex.Application.Recipients.Commands.DeleteRecipient;
using Prezentex.Application.Recipients.Queries.GetAllRecipients;
using Prezentex.Application.Recipients.Queries.GetRecipient;
using Prezentex.Domain.Entities;

namespace Application.UnitTests.HandlerTests
{
    public class RecipientsHandlersTests : TestBase
    {
        //naming convention: UnitOfWork_StateUnderTest_ExpectedBehavior

        private readonly Mock<IRecipientsRepository> _repositoryStub = new();

        [Fact]
        public async Task GetRecipient_WithUnexistingRecipient_ThrowResourceNotFoundException()
        {
            //Arrange
            _repositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Recipient)null);
            var recipientId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var query = new GetRecipientQuery(recipientId, userId);
            var handler = new GetRecipientHandler(_repositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(query, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task GetRecipient_WithExistingRecipient_ReturnsExpectedRecipient()
        {
            //Arrange
            var expectedRecipient = CreateRandomRecipient();
            _repositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedRecipient);
            var query = new GetRecipientQuery(expectedRecipient.Id, expectedRecipient.UserId);
            var handler = new GetRecipientHandler(_repositoryStub.Object);

            //Act
            var result = await handler.Handle(query, _cancellationToken);

            //Assert
            result.Should().BeEquivalentTo(
                expectedRecipient,
                options => options
                    .ExcludingMissingMembers());
        }

        [Fact]
        public async Task GetAllRecipients_WithUnexistingRecipients_ReturnsEmptyArray()
        {
            //Arrange
            _repositoryStub.Setup(repo => repo.GetRecipientsAsync())
                .ReturnsAsync(new Recipient[0]);
            var query = new GetAllRecipientsQuery();
            var handler = new GetAllRecipientsHandler(_repositoryStub.Object);

            //Act
            var result = await handler.Handle(query, _cancellationToken);

            //Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllRecipients_WithExistingRecipients_ReturnsArrayWithRecipients()
        {
            //Arrange
            var expectedArray = new Recipient[]
            {
                CreateRandomRecipient(),
                CreateRandomRecipient()
            };
            _repositoryStub.Setup(repo => repo.GetRecipientsAsync())
                .ReturnsAsync(expectedArray);
            var query = new GetAllRecipientsQuery();
            var handler = new GetAllRecipientsHandler(_repositoryStub.Object);

            //Act
            var result = await handler.Handle(query, _cancellationToken);

            //Assert
            result.Should().BeEquivalentTo(
                expectedArray,
                options => options
                    .ExcludingMissingMembers());
        }

        [Fact]
        public async Task CreateRecipient_WithRecipientToCreate_ReturnsCreatedRecipient()
        {
            //Arrange
            var command = new CreateRecipientCommand(
                Guid.NewGuid().ToString(),
                DateTimeOffset.UtcNow.Date,
                DateTimeOffset.UtcNow.Date,
                Guid.NewGuid().ToString(),
                Guid.NewGuid());
            var handler = new CreateRecipientHandler(_repositoryStub.Object);

            //Act
            var result = await handler.Handle(command, _cancellationToken);

            //Assert
            command.Should().BeEquivalentTo(
                result,
                options => options
                .ComparingByMembers<Recipient>()
                .ExcludingMissingMembers());
            result.Id.Should().NotBeEmpty();
            result.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, new TimeSpan(0, 0, 1));
        }

        [Fact]
        public async Task UpdateRecipient_WithExistingRecipient_ReturnsModifiedRecipient()
        {
            //Arrange
            var existingRecipient = CreateRandomRecipient();
            var recipientId = existingRecipient.Id;
            _repositoryStub.Setup(options => options.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingRecipient);
            var command = new UpdateRecipientCommand(
                existingRecipient.Id,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                DateTimeOffset.UtcNow.Date,
                DateTimeOffset.UtcNow.Date,
                existingRecipient.UserId);
            var handler = new UpdateRecipientHandler(_repositoryStub.Object);

            //Act
            var result = await handler.Handle(command, _cancellationToken);

            //Assert
            result.Should().BeEquivalentTo(
                command,
                options => options
                .ComparingByMembers<Recipient>()
                .ExcludingMissingMembers());
        }

        [Fact]
        public async Task UpdateRecipient_WithUnexistingRecipient_ThrowsResourceNotFoundException()
        {
            //Arrange
            var recipientId = Guid.NewGuid();
            _repositoryStub.Setup(options => options.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Recipient)null);
            var command = new UpdateRecipientCommand(
                Guid.NewGuid(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                DateTimeOffset.UtcNow.Date,
                DateTimeOffset.UtcNow.Date,
                Guid.NewGuid());
            var handler = new UpdateRecipientHandler(_repositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task DeleteRecipient_WithExistingRecipient_ReturnsUnit()
        {
            //Arrange
            var existingRecipient = CreateRandomRecipient();
            _repositoryStub.Setup(options => options.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingRecipient);
            var command = new DeleteRecipientCommand(existingRecipient.Id, existingRecipient.UserId);
            var handler = new DeleteRecipientHandler(_repositoryStub.Object);

            //Act
            var result = await handler.Handle(command, _cancellationToken);

            //Assert
            result.Should().BeOfType<Unit>();
        }

        [Fact]
        public async Task DeleteRecipient_WithUnexistingRecipient_ThrowsResourceNotFoundException()
        {
            //Arrange
            var recipientId = Guid.NewGuid();
            _repositoryStub.Setup(options => options.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Recipient)null);
            var command = new DeleteRecipientCommand(recipientId, Guid.NewGuid());
            var handler = new DeleteRecipientHandler(_repositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task DeleteRecipient_WithIncorrectUserId_ThrowsArgumentException()
        {
            //Arrange
            var existingRecipient = CreateRandomRecipient();
            _repositoryStub.Setup(options => options.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingRecipient);
            var command = new DeleteRecipientCommand(existingRecipient.Id, Guid.NewGuid());
            var handler = new DeleteRecipientHandler(_repositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<ArgumentException>();
        }
    }
}
