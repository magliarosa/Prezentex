using Prezentex.Application.Common.Interfaces.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prezentex.Api.Controllers;
using Prezentex.Api.Dtos;
using Prezentex.Api.Services;
using Prezentex.Domain.Entities;
using Prezentex.Domain.Identity;
using Prezentex.Api.DTOs;
using Microsoft.AspNetCore.Http;

namespace Api.UnitTests.ControllersTests.Identity
{
    public class IdentityControllerTests : TestBase
    {
        //naming convention: UnitOfWork_StateUnderTest_ExpectedBehavior

        private readonly Mock<IIdentityService> _identityServiceStub = new();
        private readonly Mock<IUserStore<User>> _userStoreStub;
        private readonly Mock<UserManager<User>> _userManagerStub;
        private readonly Mock<HttpContext> _httpContextStub = new();

        public IdentityControllerTests()
        {
            _userStoreStub = new();
            _userManagerStub = new(_userStoreStub.Object, null, null, null, null, null, null, null, null);
        }

        [Fact]
        public async Task LoginWithPassword_WithCorrectPassword_ReturnsAuthSuccessResponse()
        {
            //Arrange
            var loginDto = new LoginDto(
                "test@test.com",
                "Pa$$w0rd");
            var authenticationResult = new AuthenticationResult()
            {
                Success = true,
                RefreshToken = Guid.NewGuid().ToString(),
                Token = Guid.NewGuid().ToString()
            };
            _identityServiceStub.Setup(service => service.LoginWithPasswordAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(authenticationResult);
            var controller = new IdentityController(_identityServiceStub.Object, _userManagerStub.Object);

            //Act
            var result = await controller.Login(loginDto);

            //Assert
            var okResult = (result.Result as OkObjectResult);
            okResult.Should().BeOfType<OkObjectResult>();
            okResult.Value.Should().BeOfType<AuthSuccessResponse>();
        }

        [Fact]
        public async Task RegisterWithPassword_WithCorrectPassword_ReturnsAuthSuccessResponse()
        {
            //Arrange
            var registerDto = new RegisterDto(
                "test@test.com,",
                "Pa$$w0rd",
                "dispName",
                "usrname");
            var authenticationResult = new AuthenticationResult()
            {
                Success = true,
                RefreshToken = Guid.NewGuid().ToString(),
                Token = Guid.NewGuid().ToString()
            };
            _identityServiceStub.Setup(service => service.RegisterWithPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(authenticationResult);
            var controller = new IdentityController(_identityServiceStub.Object, _userManagerStub.Object);

            //Act
            var result = await controller.Register(registerDto);

            //Assert
            var okResult = (result.Result as OkObjectResult);
            okResult.Should().BeOfType<OkObjectResult>();
            okResult.Value.Should().BeOfType<AuthSuccessResponse>();
        }

        [Fact]
        public async Task GetCurrentUser_WithCorrectToken_ReturnsLoggedUserDto()
        {
            //Arrange
            var loggedUser = new LoggedUserDto(
                "dspname",
                "test@test.com",
                null,
                "usrname");
            _userManagerStub.Setup(service => service.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User
                {
                    DisplayName = loggedUser.DisplayName,
                    Email = loggedUser.Email,
                    UserName = loggedUser.UserName
                });
            _httpContextStub.Setup(context => context.User)
                .Returns(GenerateClaimsPrincipal(Guid.NewGuid()));
            var controller = new IdentityController(_identityServiceStub.Object, _userManagerStub.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = _httpContextStub.Object
            };

            //Act
            var result = await controller.GetCurrentUser();

            //Assert
            var okResult = (result.Result as OkObjectResult);
            okResult.Should().BeOfType<OkObjectResult>();
            okResult.Value.Should().BeOfType<LoggedUserDto>();
            okResult.Value.Should().BeEquivalentTo(loggedUser);
        }

        [Fact]
        public async Task FacebookAuth_WithCorrectAccessToken_ReturnsAuthSuccessResponse()
        {
            //Arrange
            var facebookRequest = new UserFacebookAuthRequestDto(
                Guid.NewGuid().ToString());
            var authenticationResult = new AuthenticationResult()
            {
                Success = true,
                RefreshToken = Guid.NewGuid().ToString(),
                Token = Guid.NewGuid().ToString()
            };
            _identityServiceStub.Setup(service => service.LoginWithFacebookAsync(It.IsAny<string>()))
                .ReturnsAsync(authenticationResult);
            var controller = new IdentityController(_identityServiceStub.Object, _userManagerStub.Object);

            //Act
            var result = await controller.FacebookAuth(facebookRequest);

            //Assert
            var okResult = (result as OkObjectResult);
            okResult.Should().BeOfType<OkObjectResult>();
            okResult.Value.Should().BeOfType<AuthSuccessResponse>();
        }

        [Fact]
        public async Task FacebookAuth_WithInCorrectAccessToken_ReturnsAuthSuccessResponse()
        {
            //Arrange
            var facebookRequest = new UserFacebookAuthRequestDto(
                Guid.NewGuid().ToString());
            var authenticationResult = new AuthenticationResult()
            {
                Success = false,
                Errors = new List<string>() { "error" }
            };
            _identityServiceStub.Setup(service => service.LoginWithFacebookAsync(It.IsAny<string>()))
                .ReturnsAsync(authenticationResult);
            var controller = new IdentityController(_identityServiceStub.Object, _userManagerStub.Object);

            //Act
            var result = await controller.FacebookAuth(facebookRequest);

            //Assert
            var badRequestResult = (result as BadRequestObjectResult);
            badRequestResult.Should().BeOfType<BadRequestObjectResult>();
            badRequestResult.Value.Should().BeOfType<AuthFailedResponse>();
        }

        [Fact]
        public async Task AuthRefresh_WithCorrectRefreshToken_ReturnsAuthSuccessResponse()
        {
            //Arrange
            var refreshTokenRequest = new RefreshTokenRequestDto(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());
            var authenticationResult = new AuthenticationResult()
            {
                Success = true,
                RefreshToken = Guid.NewGuid().ToString(),
                Token = Guid.NewGuid().ToString()
            };
            _identityServiceStub.Setup(
                service => service.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(authenticationResult);
            var controller = new IdentityController(_identityServiceStub.Object, _userManagerStub.Object);

            //Act
            var result = await controller.AuthRefresh(refreshTokenRequest);

            //Assert
            var okResult = (result as OkObjectResult);
            okResult.Should().BeOfType<OkObjectResult>();
            okResult.Value.Should().BeOfType<AuthSuccessResponse>();
        }

        [Fact]
        public async Task AuthRefresh_WithIncorrectRefreshToken_ReturnsBadRequestObjectResult()
        {
            //Arrange
            var refreshTokenRequest = new RefreshTokenRequestDto(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());
            var authenticationResult = new AuthenticationResult()
            {
                Success = false,
                Errors = new List<string>() { "error" }
            };
            _identityServiceStub.Setup(
                service => service.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(authenticationResult);
            var controller = new IdentityController(_identityServiceStub.Object, _userManagerStub.Object);

            //Act
            var result = await controller.AuthRefresh(refreshTokenRequest);

            //Assert
            var badRequestResult = (result as BadRequestObjectResult);
            badRequestResult.Should().BeOfType<BadRequestObjectResult>();
            badRequestResult.Value.Should().BeOfType<AuthFailedResponse>();
        }

    }
}
