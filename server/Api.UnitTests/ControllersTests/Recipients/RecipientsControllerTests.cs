using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prexentex.Application.Recipients.Commands.UpdateRecipient;
using Prezentex.Api.Controllers;
using Prezentex.Api.Dtos;
using Prezentex.Application.Recipients.Commands.CreateRecipient;
using Prezentex.Application.Recipients.Commands.DeleteRecipient;
using Prezentex.Application.Recipients.Queries.GetAllRecipients;
using Prezentex.Application.Recipients.Queries.GetRecipient;
using Prezentex.Domain.Entities;

namespace Api.UnitTests.ControllersTests.Recipients
{
    public class RecipientsControllerTests : TestBase
    {
        //naming convention: UnitOfWork_StateUnderTest_ExpectedBehavior

        private readonly Random rand = new Random();
        private readonly Mock<HttpContext> _httpContextStub = new();
        private readonly Mock<IMediator> _mediatorStub = new();

        [Fact]
        public async Task GetRecipientAsync_WithExistingRecipient_ReturnsOkObjectResult()
        {
            //Arrange
            var expectedRecipient = CreateRandomRecipient();
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<GetRecipientQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedRecipient);
            _httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(expectedRecipient.UserId));
            var controllerContext = new ControllerContext()
            {
                HttpContext = _httpContextStub.Object
            };
            var controller = new RecipientsController(_mediatorStub.Object);
            controller.ControllerContext = controllerContext;

            //Act
            var result = await controller.GetRecipientAsync(Guid.NewGuid());

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }


        [Fact]
        public async Task GetRecipientsAsync_WithExistingRecipients_ReturnsOkObjectResult()
        {
            //Arrange
            var expectedArray = new Recipient[]
            {
                CreateRandomRecipient(),
                CreateRandomRecipient()
            };
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<GetAllRecipientsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedArray);
            var controller = new RecipientsController(_mediatorStub.Object);

            //Act
            var result = await controller.GetRecipientsAsync();

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task CreateRecipientAsync_WithRecipientToCreate_ReturnsCreatedAtActionResult()
        {
            //Arrange
            var expectedRecipient = CreateRandomRecipient();
            var recipientToCreate = new CreateRecipientDto(
                expectedRecipient.Name,
                expectedRecipient.Note,
                expectedRecipient.BirthDay,
                expectedRecipient.NameDay);
            var controller = new RecipientsController(_mediatorStub.Object);
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<CreateRecipientCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedRecipient);
            _httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(Guid.NewGuid()));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = _httpContextStub.Object
            };

            //Act
            var result = await controller.CreateRecipientAsync(recipientToCreate);

            //Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
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
        public async Task UpdateRecipientAsync_WithExistingRecipient_ReturnsOkObjectResult()
        {
            //Arrange
            var expectedRecipient = CreateRandomRecipient();
            var recipientToUpdate = new UpdateRecipientDto(
                expectedRecipient.Name,
                expectedRecipient.Note,
                expectedRecipient.BirthDay,
                expectedRecipient.NameDay);
          

            var recipientId = expectedRecipient.Id;
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<UpdateRecipientCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedRecipient);
            var controller = new RecipientsController(_mediatorStub.Object);
            _httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(expectedRecipient.UserId));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = _httpContextStub.Object
            };

            //Act
            var result = await controller.UpdateRecipientAsync(recipientId, recipientToUpdate);

            //Assert
            var updatedRecipient = (result.Result as OkObjectResult).Value as RecipientDto;
            updatedRecipient.Should().BeEquivalentTo(recipientToUpdate);
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteRecipientAsync_WithExistingRecipient_ReturnsNoContentResult()
        {
            //Arrange
            var existingRecipient = CreateRandomRecipient();
            var recipientId = existingRecipient.Id;
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<DeleteRecipientCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);
            var controller = new RecipientsController(_mediatorStub.Object);
            _httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(existingRecipient.UserId));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = _httpContextStub.Object
            };

            //Act
            var result = await controller.DeleteRecipientAsync(recipientId);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
