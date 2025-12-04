using Luftfartshinder.Controllers;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Luftfartshinder.Tests
{
    public class UnitTestingTests
    {
        /// <summary>
        /// GOAL: Test that HomeController.Index() returns a view
        /// LOGIC: Calls Index() method and checks the return type
        /// RESULT: ViewResult should be returned
        /// </summary>
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


