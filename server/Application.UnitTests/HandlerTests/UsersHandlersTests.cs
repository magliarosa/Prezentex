using Application.UnitTests;
using FluentAssertions;
using MediatR;
using Moq;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Application.Exceptions;
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

namespace Application.UnitTests.HandlerTests
{
    public class UsersHandlersTests : TestBase
    {
        //naming convention: UnitOfWork_StateUnderTest_ExpectedBehavior

        private readonly Mock<IRecipientsRepository> recipientsRepositoryStub = new();
        private readonly Mock<IUsersRepository> usersRepositoryStub = new();
        private readonly Mock<IGiftsRepository> giftsRepositoryStub = new();

        [Fact]
        public async Task GetUser_WithUnexistingUser_ThrowsResourceNotFoundException()
        {
            //Arrange
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var userId = Guid.NewGuid();
            var query = new GetUserQuery(userId, userId);
            var handler = new GetUserHandler(usersRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(query, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task GetUser_WithWrongUser_ThrowsUnauthorizedAccessException()
        {
            //Arrange
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var userId = Guid.NewGuid();
            var query = new GetUserQuery(userId, Guid.NewGuid());
            var handler = new GetUserHandler(usersRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(query, _cancellationToken))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task GetUser_WithExistingUser_ReturnsExpectedUser()
        {
            //Arrange
            var expectedUser = CreateRandomUser();
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedUser);
            var query = new GetUserQuery(expectedUser.Id, expectedUser.Id);
            var handler = new GetUserHandler(usersRepositoryStub.Object);

            //Act
            var result = await handler.Handle(query, _cancellationToken);

            //Assert
            result.Should().BeEquivalentTo(
                expectedUser,
                options => options
                    .ExcludingMissingMembers());
        }

        [Fact]
        public async Task GetAllUsers_WithUnexistingUsers_ReturnsEmptyArray()
        {
            //Arrange
            usersRepositoryStub.Setup(repo => repo.GetUsersAsync())
                .ReturnsAsync(new List<User>());
            var query = new GetAllUsersQuery();
            var handler = new GetAllUsersHandler(usersRepositoryStub.Object);

            //Act
            var result = await handler.Handle(query, _cancellationToken);

            //Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllUsers_WithExistingUsers_ReturnsArrayWithUsers()
        {
            //Arrange
            var expectedArray = new List<User>
            {
                CreateRandomUser(),
                CreateRandomUser()
            };
            usersRepositoryStub.Setup(repo => repo.GetUsersAsync())
                .ReturnsAsync(expectedArray);
            var query = new GetAllUsersQuery();
            var handler = new GetAllUsersHandler(usersRepositoryStub.Object);

            //Act
            var result = await handler.Handle(query, _cancellationToken);

            //Assert
            result.Should().BeEquivalentTo(
                expectedArray,
                options => options
                    .ExcludingMissingMembers());
        }

        [Fact]
        public async Task CreateUser_WithUserToCreate_ReturnsCreatedUser()
        {
            //Arrange
            var command = new CreateUserCommand(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());
            var handler = new CreateUserHandler(usersRepositoryStub.Object);

            //Act
            var result = await handler.Handle(command, _cancellationToken);

            //Assert
            command.Should().BeEquivalentTo(
                result,
                options => options
                .ComparingByMembers<User>()
                .ExcludingMissingMembers());
            result.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task UpdateUser_WithExistingUser_ReturnsModifiedUser()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userId = existingUser.Id;
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            var command = new UpdateUserCommand(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                existingUser.Id,
                existingUser.Id);
            var handler = new UpdateUserHandler(usersRepositoryStub.Object);

            //Act
            var result = await handler.Handle(command, _cancellationToken);

            //Assert
            result.Should().BeEquivalentTo(
                command,
                options => options
                .ComparingByMembers<User>()
                .ExcludingMissingMembers());
        }

        [Fact]
        public async Task UpdateUser_WithUnexistingUser_ReturnsRosourceNotFoundException()
        {
            //Arrange
            var userId = Guid.NewGuid();
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var command = new UpdateUserCommand(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                userId,
                userId);
            var handler = new UpdateUserHandler(usersRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task UpdateUser_WithWrongUser_ReturnsUnauthorizedAccessException()
        {
            //Arrange
            var userId = Guid.NewGuid();
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var command = new UpdateUserCommand(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                userId,
                Guid.NewGuid());
            var handler = new UpdateUserHandler(usersRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task DeleteUser_WithExistingUser_ReturnsUnit()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userId = existingUser.Id;
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            var command = new DeleteUserCommand(existingUser.Id, existingUser.Id);
            var handler = new DeleteUserHandler(usersRepositoryStub.Object);

            //Act
            var result = await handler.Handle(command, _cancellationToken);

            //Assert
            result.Should().BeOfType<Unit>();
        }

        [Fact]
        public async Task DeleteUser_WithUnexistingUser_ThrowsResourceNotFoundException()
        {
            //Arrange
            var userId = Guid.NewGuid();
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var command = new DeleteUserCommand(userId, userId);
            var handler = new DeleteUserHandler(usersRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task DeleteUser_WithWrongUser_ThrowsUnauthorizedAccessException()
        {
            //Arrange
            var userId = Guid.NewGuid();
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var command = new DeleteUserCommand(userId, Guid.NewGuid());
            var handler = new DeleteUserHandler(usersRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task AddGiftToUser_WithExistingUserAndGift_ReturnsUnit()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userId = existingUser.Id;
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            var existingGift = CreateRandomGift();
            existingGift.UserId = userId;
            var giftId = existingGift.Id;
            giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingGift);
            var command = new AddGiftToUserCommand(giftId, userId, userId);
            var handler = new AddGiftToUserHandler(usersRepositoryStub.Object, giftsRepositoryStub.Object);

            //Act
            var result = await handler.Handle(command, _cancellationToken);

            //Assert
            result.Should().BeOfType<Unit>();
        }

        [Fact]
        public async Task AddGiftToUser_WithUnexistingGift_ThrowsResourceNotFoundException()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userId = existingUser.Id;
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            var giftId = Guid.NewGuid();
            giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Gift)null);
            var command = new AddGiftToUserCommand(giftId, userId, userId);
            var handler = new AddGiftToUserHandler(usersRepositoryStub.Object, giftsRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task AddGiftToUser_WithUnexistingUser_ThrowsResourceNotFoundException()
        {
            //Arrange
            var userId = Guid.NewGuid();
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var existingGift = CreateRandomGift();
            var giftId = existingGift.Id;
            giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingGift);
            var command = new AddGiftToUserCommand(giftId, userId, userId);
            var handler = new AddGiftToUserHandler(usersRepositoryStub.Object, giftsRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }


        [Fact]
        public async Task RemoveGiftFromUser_WithExistingUserAndGift_ReturnsUnit()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userId = existingUser.Id;
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            var existingGift = CreateRandomGift();
            var giftId = existingGift.Id;
            existingGift.UserId = userId;
            giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingGift);
            existingUser.Gifts.Add(existingGift);
            var command = new RemoveGiftFromUserCommand(giftId, userId, userId);
            var handler = new RemoveGiftFromUserHandler(usersRepositoryStub.Object, giftsRepositoryStub.Object);

            //Act
            var result = await handler.Handle(command, _cancellationToken);

            //Assert
            result.Should().BeOfType<Unit>();
        }

        [Fact]
        public async Task RemoveGiftFromUser_WithUnexistingGift_ThrowsResourceNotFoundException()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userId = existingUser.Id;
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            var giftId = Guid.NewGuid();
            giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Gift)null);
            var command = new RemoveGiftFromUserCommand(giftId, userId, userId);
            var handler = new RemoveGiftFromUserHandler(usersRepositoryStub.Object, giftsRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task RemoveGiftFromUser_WithUnexistingUser_ThrowsResourceNotFoundException()
        {
            //Arrange
            var userId = Guid.NewGuid();
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var existingGift = CreateRandomGift();
            var giftId = existingGift.Id;
            giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingGift);
            var command = new RemoveGiftFromUserCommand(giftId, userId, userId);
            var handler = new RemoveGiftFromUserHandler(usersRepositoryStub.Object, giftsRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task AddRecipientToUser_WithExistingUserAndRecipient_ReturnsUnit()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userId = existingUser.Id;
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            var existingRecipient = CreateRandomRecipient();
            var recipientId = existingRecipient.Id;
            existingRecipient.UserId = userId;
            recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingRecipient);
            var command = new AddRecipientToUserCommand(recipientId, userId, userId);
            var handler = new AddRecipientToUserHandler(usersRepositoryStub.Object, recipientsRepositoryStub.Object);

            //Act
            var result = await handler.Handle(command, _cancellationToken);

            //Assert
            result.Should().BeOfType<Unit>();
        }

        [Fact]
        public async Task AddRecipientToUser_WithUnexistingRecipient_ThrowsResourceNotFoundException()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userId = existingUser.Id;
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            var recipientId = Guid.NewGuid();
            recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Recipient)null);
            var command = new AddRecipientToUserCommand(recipientId, userId, userId);
            var handler = new AddRecipientToUserHandler(usersRepositoryStub.Object, recipientsRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task AddRecipientToUser_WithUnexistingUser_ThrowsResourceNotFoundException()
        {
            //Arrange
            var userId = Guid.NewGuid();
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var existingRecipient = CreateRandomRecipient();
            var recipientId = existingRecipient.Id;
            recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingRecipient);
            var command = new AddRecipientToUserCommand(recipientId, userId, userId);
            var handler = new AddRecipientToUserHandler(usersRepositoryStub.Object, recipientsRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task RemoveRecipientFromUser_WithExistingUserAndRecipient_ReturnsUnit()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userId = existingUser.Id;
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            var existingRecipient = CreateRandomRecipient();
            var recipientId = existingRecipient.Id;
            existingRecipient.UserId = userId;
            recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingRecipient);
            existingUser.Recipients.Add(existingRecipient);
            var command = new RemoveRecipientFromUserCommand(recipientId, userId, userId);
            var handler = new RemoveRecipientFromUserHandler(usersRepositoryStub.Object, recipientsRepositoryStub.Object);

            //Act
            var result = await handler.Handle(command, _cancellationToken);

            //Assert
            result.Should().BeOfType<Unit>();
        }

        [Fact]
        public async Task RemoveRecipientFromUser_WithUnexistingRecipient_ThrowsResourceNotFoundException()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userId = existingUser.Id;
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            var recipientId = Guid.NewGuid();
            recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Recipient)null);
            var command = new RemoveRecipientFromUserCommand(recipientId, userId, userId);
            var handler = new RemoveRecipientFromUserHandler(usersRepositoryStub.Object, recipientsRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task RemoveRecipientFromUser_WithUnexistingUser_ThrowsResourceNotFoundException()
        {
            //Arrange
            var userId = Guid.NewGuid();
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var existingRecipient = CreateRandomRecipient();
            var recipientId = existingRecipient.Id;
            recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingRecipient);
            var command = new RemoveRecipientFromUserCommand(recipientId, userId, userId);
            var handler = new RemoveRecipientFromUserHandler(usersRepositoryStub.Object, recipientsRepositoryStub.Object);

            //Assert
            await handler.Invoking(x => x.Handle(command, _cancellationToken))
                .Should().ThrowAsync<ResourceNotFoundException>();
        }
    }
}
