using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prezentex.Api.Controllers;
using Prezentex.Api.Dtos;
using Prezentex.Api.Services.Identity;
using Prezentex.Domain.Identity;

namespace Api.UnitTests.ControllersTests.Identity
{
    public class IdentityControllerTests
    {
        //naming convention: UnitOfWork_StateUnderTest_ExpectedBehavior

        private readonly Mock<IIdentityService> _identityServiceStub = new();

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
            var controller = new IdentityController(_identityServiceStub.Object);

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
            var controller = new IdentityController(_identityServiceStub.Object);

            //Act
            var result = await controller.FacebookAuth(facebookRequest);

            //Assert
            var badRequestResult = (result as BadRequestObjectResult);
            badRequestResult.Should().BeOfType<BadRequestObjectResult>();
            badRequestResult.Value.Should().BeOfType<AuthFailedResponse>();
        }

        [Fact]
        public async Task FacebookAuthRefresh_WithCorrectRefreshToken_ReturnsAuthSuccessResponse()
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
            var controller = new IdentityController(_identityServiceStub.Object);

            //Act
            var result = await controller.FacebookAuthRefresh(refreshTokenRequest);

            //Assert
            var okResult = (result as OkObjectResult);
            okResult.Should().BeOfType<OkObjectResult>();
            okResult.Value.Should().BeOfType<AuthSuccessResponse>();
        }

        [Fact]
        public async Task FacebookAuthRefresh_WithIncorrectRefreshToken_ReturnsBadRequestObjectResult()
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
            var controller = new IdentityController(_identityServiceStub.Object);

            //Act
            var result = await controller.FacebookAuthRefresh(refreshTokenRequest);

            //Assert
            var badRequestResult = (result as BadRequestObjectResult);
            badRequestResult.Should().BeOfType<BadRequestObjectResult>();
            badRequestResult.Value.Should().BeOfType<AuthFailedResponse>();
        }

    }
}
