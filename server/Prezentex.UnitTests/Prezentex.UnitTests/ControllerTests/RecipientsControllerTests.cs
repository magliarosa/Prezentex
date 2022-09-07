using Microsoft.AspNetCore.Mvc;
using Moq;
using Prezentex.Api.Dtos;
using Prezentex.Api.Entities;
using Prezentex.Api.Repositories.Recipients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Prezentex.UnitTests.ControllerTests
{
    public class RecipientsControllerTests
    {
        //naming convention: UnitOfWork_StateUnderTest_ExpectedBehavior

        private readonly Mock<IRecipientsRepository> repositoryStub = new();
        private readonly Random rand = new Random();

        [Fact]
        public async Task GetRecipientAsync_WithUnexistingRecipient_ReturnsNotFound()
        {
            //Arrange
            repositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Recipient)null);

            var controller = new RecipientsController(repositoryStub.Object);

            //Act
            var result = await controller.GetRecipientAsync(Guid.NewGuid());

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetRecipientAsync_WithExistingRecipient_ReturnsExpectedGift()
        {
            //Arrange
            var expectedRecipient = CreateRandomRecipient();
            repositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedRecipient);
            var controller = new RecipientsController(repositoryStub.Object);

            //Act
            var result = await controller.GetRecipientAsync(Guid.NewGuid());

            //Assert
            result.Value.Should().BeEquivalentTo(
                expectedRecipient,
                options => options
                    .ExcludingMissingMembers());
        }

        [Fact]
        public async Task GetRecipientsAsync_WithUnexistingRecipients_ReturnsEmptyArray()
        {
            //Arrange
            repositoryStub.Setup(repo => repo.GetRecipientsAsync())
                .ReturnsAsync(new Recipient[0]);
            var controller = new RecipientsController(repositoryStub.Object);

            //Act
            var result = await controller.GetRecipientsAsync();

            //Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetRecipientsAsync_WithExistingRecipients_ReturnsArrayWithRecipients()
        {
            //Arrange
            var expectedArray = new Recipient[]
            {
                CreateRandomRecipient(),
                CreateRandomRecipient()
            };
            repositoryStub.Setup(repo => repo.GetRecipientsAsync())
                .ReturnsAsync(expectedArray);
            var controller = new RecipientsController(repositoryStub.Object);

            //Act
            var result = await controller.GetRecipientsAsync();

            //Assert
            result.Should().BeEquivalentTo(
                expectedArray,
                options => options
                    .ExcludingMissingMembers());
        }

        [Fact]
        public async Task CreateRecipientAsync_WithRecipientToCreate_ReturnsCreatedRecipient()
        {
            //Arrange
            var recipientToCreate = new CreateRecipientDto(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                rand.Next(2000),
                Guid.NewGuid().ToString());
            var controller = new RecipientController(repositoryStub.Object);

            //Act
            var result = await controller.CreateRecipientAsync(recipientToCreate);

            //Assert
            var createdRecipient = (result.Result as CreatedAtActionResult).Value as RecipientDto;
            recipientToCreate.Should().BeEquivalentTo(
                createdRecipient,
                options => options
                .ComparingByMembers<RecipientDto>()
                .ExcludingMissingMembers());
            createdRecipient.Id.Should().NotBeEmpty();
            createdRecipient.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, new TimeSpan(0, 0, 1));
        }

        [Fact]
        public async Task UpdateRecipientAsync_WithExistingRecipient_ReturnsModifiedRecipient()
        {
            //Arrange
            var existingRecipient = CreateRandomRecipient();
            var recipientToUpdate = new UpdateRecipientDto(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                rand.Next(1000),
                Guid.NewGuid().ToString());
            var recipientId = existingRecipient.Id;
            repositoryStub.Setup(options => options.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingRecipient);
            var controller = new RecipientController(repositoryStub.Object);

            //Act
            var result = await controller.UpdateRecipientAsync(recipientId, recipientToUpdate);

            //Assert
            var updatedRecipient = (result.Result as OkObjectResult).Value as RecipientDto;
            updatedRecipient.Should().BeEquivalentTo(recipientToUpdate);
        }

        [Fact]
        public async Task UpdateRecipientAsync_WithUnexistingRecipient_ReturnsNotFound()
        {
            //Arrange
            var recipientToUpdate = new UpdateRecipientDto(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                rand.Next(1000),
                Guid.NewGuid().ToString());
            var recipientId = Guid.NewGuid();
            repositoryStub.Setup(options => options.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Recipient)null);
            var controller = new RecipientsController(repositoryStub.Object);

            //Act
            var result = await controller.UpdateRecipientAsync(recipientId, recipientToUpdate);

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteRecipientAsync_WithExistingRecipient_ReturnsNoContent()
        {
            //Arrange
            var existingRecipient = CreateRandomRecipient();
            var recipientId = existingRecipient.Id;
            repositoryStub.Setup(options => options.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingRecipient);
            var controller = new GiftsRecipient(repositoryStub.Object);

            //Act
            var result = await controller.DeleteRecipientAsync(recipientId);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        public async Task DeleteRecipientAsync_WithUnexistingRecipient_ReturnsNotFound()
        {
            //Arrange
            var recipientId = Guid.NewGuid();
            repositoryStub.Setup(options => options.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Recipient)null);
            var controller = new RecipientsController(repositoryStub.Object);

            //Act
            var result = await controller.DeleteRecipientAsync(recipientId);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        private Recipient CreateRandomRecipient()
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
