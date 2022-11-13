using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prezentex.Api.Controllers;
using Prezentex.Api.Dtos;
using Prezentex.Api.Entities;
using Prezentex.Api.Repositories.Recipients;
using System;
using System.Threading.Tasks;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prezentex.Api.Repositories;
using Microsoft.AspNetCore.Http;

namespace Prezentex.UnitTests.ControllerTests
{
    public class UsersControllerTests : TestBase
    {
        //naming convention: UnitOfWork_StateUnderTest_ExpectedBehavior

        private readonly Mock<IRecipientsRepository> recipientsRepositoryStub = new();
        private readonly Mock<IUsersRepository> usersRepositoryStub = new();
        private readonly Mock<IGiftsRepository> giftsRepositoryStub = new();
        private readonly Mock<HttpContext> httpContextStub = new();

        [Fact]
        public async Task GetUserAsync_WithUnexistingUser_ReturnsNotFound()
        {
            //Arrange
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);

            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);

            //Act
            var result = await controller.GetUserAsync(Guid.NewGuid());

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetUserAsync_WithExistingUser_ReturnsExpectedUser()
        {
            //Arrange
            var expectedUser = CreateRandomUser();
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedUser);
            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);
            httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(expectedUser.Id));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextStub.Object
            };

            //Act
            var result = await controller.GetUserAsync(Guid.NewGuid());

            //Assert
            result.Value.Should().BeEquivalentTo(
                expectedUser,
                options => options
                    .ExcludingMissingMembers());
        }

        [Fact]
        public async Task GetUsersAsync_WithUnexistingUsers_ReturnsEmptyArray()
        {
            //Arrange
            usersRepositoryStub.Setup(repo => repo.GetUsersAsync())
                .ReturnsAsync(new List<User>());

            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);

            //Act
            var result = await controller.GetUsersAsync();

            //Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetUsersAsync_WithExistingUsers_ReturnsArrayWithUsers()
        {
            //Arrange
            var expectedArray = new List<User>
            {
                CreateRandomUser(),
                CreateRandomUser()
            };
            usersRepositoryStub.Setup(repo => repo.GetUsersAsync())
                .ReturnsAsync(expectedArray);
            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);

            //Act
            var result = await controller.GetUsersAsync();

            //Assert
            result.Should().BeEquivalentTo(
                expectedArray,
                options => options
                    .ExcludingMissingMembers());
        }

        [Fact]
        public async Task CreateUserAsync_WithUserToCreate_ReturnsCreatedUser()
        {
            //Arrange
            var userToCreate = new CreateUserDto(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());
            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);

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
        }

        [Fact]
        public async Task UpdateUserAsync_WithExistingUser_ReturnsModifiedUser()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userToUpdate = new UpdateUserDto(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());

            var recipientId = existingUser.Id;
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);
            httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(existingUser.Id));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextStub.Object
            };

            //Act
            var result = await controller.UpdateUserAsync(recipientId, userToUpdate);

            //Assert
            var updatedUser = (result.Result as OkObjectResult).Value as UserDto;
            updatedUser.Should().BeEquivalentTo(userToUpdate);
        }

        [Fact]
        public async Task UpdateUserAsync_WithUnexistingUser_ReturnsNotFound()
        {
            //Arrange
            var userToUpdate = new UpdateUserDto(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());
            var userId = Guid.NewGuid();
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);

            //Act
            var result = await controller.UpdateUserAsync(userId, userToUpdate);

            //Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteUserAsync_WithExistingUser_ReturnsNoContent()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userId = existingUser.Id;
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);
            httpContextStub.Setup(context => context.User)
               .Returns(GenerateClaimsPrincipal(existingUser.Id));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextStub.Object
            };

            //Act
            var result = await controller.DeleteUserAsync(userId);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteUserAsync_WithUnexistingUser_ReturnsNotFound()
        {
            //Arrange
            var userId = Guid.NewGuid();
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);

            //Act
            var result = await controller.DeleteUserAsync(userId);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AddGiftToUserAsync_WithExistingUserAndGift_ReturnsNoContent()
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
            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);
            httpContextStub.Setup(context => context.User)
               .Returns(GenerateClaimsPrincipal(existingUser.Id));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextStub.Object
            };
            var addGiftDto = new AddGiftToUserDto(giftId);

            //Act
            var result = await controller.AddGiftToUserAsync(userId, addGiftDto);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task AddGiftToUserAsync_WithUnexistingGift_ReturnsNoContent()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userId = existingUser.Id;
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            var giftId = Guid.NewGuid();
            giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Gift)null);
            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);
            var addGiftDto = new AddGiftToUserDto(giftId);

            //Act
            var result = await controller.AddGiftToUserAsync(userId, addGiftDto);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AddGiftToUserAsync_WithUnexistingUser_ReturnsNotFound()
        {
            //Arrange
            var userId = Guid.NewGuid();
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var existingGift = CreateRandomGift();
            var giftId = existingGift.Id;
            giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingGift);
            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);
            var addGiftDto = new AddGiftToUserDto(giftId);

            //Act
            var result = await controller.AddGiftToUserAsync(userId, addGiftDto);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }


        [Fact]
        public async Task RemoveGiftFromUserAsync_WithExistingUserAndGift_ReturnsNoContent()
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
            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);
            httpContextStub.Setup(context => context.User)
               .Returns(GenerateClaimsPrincipal(userId));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextStub.Object
            };
            var removeGiftDto = new RemoveGiftFromUserDto(giftId);

            //Act
            var result = await controller.RemoveGiftFromUserAsync(userId, removeGiftDto);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }
        
        [Fact]
        public async Task RemoveGiftFromUserAsync_WithUnexistingGift_ReturnsNotFound()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userId = existingUser.Id;
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            var giftId = Guid.NewGuid();
            giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Gift)null);
            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);
            var removeGiftDto = new RemoveGiftFromUserDto(giftId);

            //Act
            var result = await controller.RemoveGiftFromUserAsync(userId, removeGiftDto);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }
        
        [Fact]
        public async Task RemoveGiftFromUserAsync_WithUnexistingUser_ReturnsNotFound()
        {
            //Arrange
            var userId = Guid.NewGuid();
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var existingGift = CreateRandomGift();
            var giftId = existingGift.Id;
            giftsRepositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingGift);
            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);
            var removeGiftDto = new RemoveGiftFromUserDto(giftId);

            //Act
            var result = await controller.RemoveGiftFromUserAsync(userId, removeGiftDto);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AddRecipientToUserAsync_WithExistingUserAndRecipient_ReturnsNoContent()
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
            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);
            httpContextStub.Setup(context => context.User)
               .Returns(GenerateClaimsPrincipal(existingUser.Id));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextStub.Object
            };
            var addRecipientDto = new AddRecipientToUserDto(recipientId);

            //Act
            var result = await controller.AddRecipientToUserAsync(userId, addRecipientDto);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }
        [Fact]
        public async Task AddRecipientToUserAsync_WithUnexistingRecipient_ReturnsNotFound()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userId = existingUser.Id;
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            var recipientId = Guid.NewGuid();
            recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Recipient)null);
            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);
            var addRecipientDto = new AddRecipientToUserDto(recipientId);

            //Act
            var result = await controller.AddRecipientToUserAsync(userId, addRecipientDto);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }
        
        [Fact]
        public async Task AddRecipientToUserAsync_WithUnexistingUser_ReturnsNotFound()
        {
            //Arrange
            var userId = Guid.NewGuid();
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var existingRecipient = CreateRandomRecipient();
            var recipientId = existingRecipient.Id;
            recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingRecipient);
            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);
            var addRecipientDto = new AddRecipientToUserDto(recipientId);

            //Act
            var result = await controller.AddRecipientToUserAsync(userId, addRecipientDto);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task RemoveRecipientFromUserAsync_WithExistingUserAndRecipient_ReturnsNoContent()
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
            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);
            httpContextStub.Setup(context => context.User)
               .Returns(GenerateClaimsPrincipal(existingUser.Id));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextStub.Object
            };
            var removeRecipientDto = new RemoveRecipientFromUserDto(recipientId);

            //Act
            var result = await controller.RemoveRecipientFromUserAsync(userId, removeRecipientDto);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }
        
        [Fact]
        public async Task RemoveRecipientFromUserAsync_WithUnexistingRecipient_ReturnsNotFound()
        {
            //Arrange
            var existingUser = CreateRandomUser();
            var userId = existingUser.Id;
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            var recipientId = Guid.NewGuid();
            recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Recipient)null);
            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);
            var removeRecipientDto = new RemoveRecipientFromUserDto(recipientId);

            //Act
            var result = await controller.RemoveRecipientFromUserAsync(userId, removeRecipientDto);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task RemoveRecipientFromUserAsync_WithUnexistingUser_ReturnsNotFound()
        {
            //Arrange
            var userId = Guid.NewGuid();
            usersRepositoryStub.Setup(repo => repo.GetUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var existingRecipient = CreateRandomRecipient();
            var recipientId = existingRecipient.Id;
            recipientsRepositoryStub.Setup(repo => repo.GetRecipientAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingRecipient);
            var controller = new UsersController(
                usersRepositoryStub.Object,
                giftsRepositoryStub.Object,
                recipientsRepositoryStub.Object);
            var removeRecipientDto = new RemoveRecipientFromUserDto(recipientId);

            //Act
            var result = await controller.RemoveRecipientFromUserAsync(userId, removeRecipientDto);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
