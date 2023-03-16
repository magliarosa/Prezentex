using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prezentex.Api.Controllers;
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
using Prezentex.Domain.Entities;

namespace Api.UnitTests.ControllersTests.Users
{
    public class UsersControllerTests : TestBase
    {
        //naming convention: UnitOfWork_StateUnderTest_ExpectedBehavior

        private readonly Random rand = new Random();
        private readonly Mock<HttpContext> _httpContextStub = new();
        private readonly Mock<IMediator> _mediatorStub = new();

        [Fact]
        public async Task GetUserAsync_WithExistingUser_ReturnsOkObjectResult()
        {
            //Arrange
            var expectedUser = CreateRandomUser();
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedUser);
            var controller = new UsersController(_mediatorStub.Object);
            _httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(expectedUser.Id));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = _httpContextStub.Object
            };

            //Act
            var result = await controller.GetUserAsync(Guid.NewGuid());

            //Assert
            var retrievedUser = (result.Result as OkObjectResult).Value as UserDto;
            retrievedUser.Should().BeEquivalentTo(
                expectedUser,
                options => options
                    .ExcludingMissingMembers());
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetUsersAsync_WithExistingUsers_ReturnsOkObjectResult()
        {
            //Arrange
            var expectedArray = new List<User>
            {
                CreateRandomUser(),
                CreateRandomUser()
            };
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedArray);
            var controller = new UsersController(_mediatorStub.Object);

            //Act
            var result = await controller.GetUsersAsync();

            //Assert
            var retrievedUsers = (result.Result as OkObjectResult).Value as IEnumerable<UserDto>;
            retrievedUsers.Should().BeEquivalentTo(
                expectedArray,
                options => options
                    .ExcludingMissingMembers());
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task CreateUserAsync_WithUserToCreate_ReturnsCreatedAtActionResult()
        {
            //Arrange
            var expectedUser = CreateRandomUser();
            var userToCreate = new CreateUserDto(
                expectedUser.Username,
                expectedUser.Email);
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedUser);
            var controller = new UsersController(_mediatorStub.Object);

            //Act
            var result = await controller.CreateUserAsync(userToCreate);

            //Assert
            var createdRecipient = (result.Result as CreatedAtActionResult).Value as UserDto;
            userToCreate.Should().BeEquivalentTo(
                createdRecipient,
                options => options
                .ComparingByMembers<UserDto>()
                .ExcludingMissingMembers());
            createdRecipient.Id.Should().NotBeEmpty();
            createdRecipient.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, new TimeSpan(0, 0, 1));
            result.Result.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task UpdateUserAsync_WithExistingUser_ReturnsOkObjectResult()
        {
            //Arrange
            var updatedUser = CreateRandomUser();
            var userToUpdate = new UpdateUserDto(
                updatedUser.Username,
                updatedUser.Email);

            var recipientId = updatedUser.Id;
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedUser);
            var controller = new UsersController(_mediatorStub.Object);
            _httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(updatedUser.Id));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = _httpContextStub.Object
            };

            //Act
            var result = await controller.UpdateUserAsync(recipientId, userToUpdate);

            //Assert
            var retrievedUser = (result.Result as OkObjectResult).Value as UserDto;
            updatedUser.Should().BeEquivalentTo(userToUpdate);
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteUserAsync_WithExistingUser_ReturnsNoContentResult()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userId = existingUser.Id;
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);
            var controller = new UsersController(_mediatorStub.Object);
            _httpContextStub.Setup(context => context.User)
               .Returns(GenerateClaimsPrincipal(existingUser.Id));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = _httpContextStub.Object
            };

            //Act
            var result = await controller.DeleteUserAsync(userId);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task AddGiftToUserAsync_WithExistingUserAndGift_ReturnsNoContentResult()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userId = existingUser.Id;
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<AddGiftToUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);
            var existingGift = CreateRandomGift();
            existingGift.UserId = userId;
            var giftId = existingGift.Id;

            var controller = new UsersController(_mediatorStub.Object);
            _httpContextStub.Setup(context => context.User)
               .Returns(GenerateClaimsPrincipal(existingUser.Id));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = _httpContextStub.Object
            };
            var addGiftDto = new AddGiftToUserDto(giftId);

            //Act
            var result = await controller.AddGiftToUserAsync(userId, addGiftDto);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task RemoveGiftFromUserAsync_WithExistingUserAndGift_ReturnsNoContent()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userId = existingUser.Id;
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<RemoveGiftFromUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);
            var existingGift = CreateRandomGift();
            var giftId = existingGift.Id;
            existingGift.UserId = userId;
            existingUser.Gifts.Add(existingGift);
            var controller = new UsersController(_mediatorStub.Object);
            _httpContextStub.Setup(context => context.User)
               .Returns(GenerateClaimsPrincipal(userId));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = _httpContextStub.Object
            };
            var removeGiftDto = new RemoveGiftFromUserDto(giftId);

            //Act
            var result = await controller.RemoveGiftFromUserAsync(userId, removeGiftDto);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task AddRecipientToUserAsync_WithExistingUserAndRecipient_ReturnsNoContentResult()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userId = existingUser.Id;
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<AddRecipientToUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);
            var existingRecipient = CreateRandomRecipient();
            var recipientId = existingRecipient.Id;
            existingRecipient.UserId = userId;
            var controller = new UsersController(_mediatorStub.Object);
            _httpContextStub.Setup(context => context.User)
               .Returns(GenerateClaimsPrincipal(existingUser.Id));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = _httpContextStub.Object
            };
            var addRecipientDto = new AddRecipientToUserDto(recipientId);

            //Act
            var result = await controller.AddRecipientToUserAsync(userId, addRecipientDto);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task RemoveRecipientFromUserAsync_WithExistingUserAndRecipient_ReturnsNoContentResult()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userId = existingUser.Id;
            _mediatorStub.Setup(mediator => mediator.Send(It.IsAny<RemoveRecipientFromUserCommand>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(Unit.Value);
            var existingRecipient = CreateRandomRecipient();
            var recipientId = existingRecipient.Id;
            existingRecipient.UserId = userId;
            existingUser.Recipients.Add(existingRecipient);
            var controller = new UsersController(_mediatorStub.Object);
            _httpContextStub.Setup(context => context.User)
               .Returns(GenerateClaimsPrincipal(existingUser.Id));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = _httpContextStub.Object
            };
            var removeRecipientDto = new RemoveRecipientFromUserDto(recipientId);

            //Act
            var result = await controller.RemoveRecipientFromUserAsync(userId, removeRecipientDto);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
