using Luftfartshinder.Controllers;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Luftfartshinder.Tests
{
    public class SystemTestingTests
    {
        /// <summary>
        /// GOAL: Test that the homepage can be loaded without errors
        /// LOGIC: Calls HomeController.Index() and verifies that a view is returned
        /// RESULT: ViewResult should be returned without null values
        /// </summary>
        [Fact]
        public void System_HomePage_LoadsSuccessfully()
        {
            // Arrange
            var obstacleRepositoryMock = new Mock<IObstacleRepository>();
            var controller = new HomeController(obstacleRepositoryMock.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
        }
    }
}


