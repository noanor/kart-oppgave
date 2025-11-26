using Luftfartshinder.Controllers;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Luftfartshinder.Tests
{
    public class UnitTestingTests
    {
        [Fact]
        public void HomeController_Index_ReturnsView()
        {
            // Arrange
            var obstacleRepositoryMock = new Mock<IObstacleRepository>();
            var controller = new HomeController(obstacleRepositoryMock.Object);

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}


