using Moq;
using Xunit;
using Prezentex.Api.Repositories;
using System;
using Prezentex.Api.Entities;
using Prezentex.Api.Controllers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Prezentex.UnitTests.ControllerTests
{
    public class GiftsControllerTests
    {

        //naming convention: UnitOfWork_StateUnderTest_ExpectedBehavior

        [Fact]
        public async Task GetGiftAsync_WithUnexistingGift_ReturnsNotFound()
        {
            //Arrange
            var repositoryStub = new Mock<IGiftsRepository>();
            repositoryStub.Setup(repo => repo.GetGiftAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Gift)null);

            var controller = new GiftsController(repositoryStub.Object);

            //Act
            var result = await controller.GetGiftAsync(Guid.NewGuid());

            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}