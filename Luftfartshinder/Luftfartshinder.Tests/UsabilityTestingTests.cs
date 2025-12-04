using Luftfartshinder.Controllers;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Luftfartshinder.Tests
{
    public class UsabilityTestingTests
    {
        /// <summary>
        /// GOAL: Test that Privacy page is accessible and returns a view
        /// LOGIC: Calls Privacy() and verifies that a view is returned
        /// RESULT: ViewResult should be returned, indicating that the page is accessible to users
        /// </summary>
        [Fact]
        public void Usability_PrivacyPage_ReturnsView()
        {
            // Arrange
            var obstacleRepositoryMock = new Mock<IObstacleRepository>();
            var controller = new HomeController(obstacleRepositoryMock.Object);

            // Act
            var result = controller.Privacy();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
        }
    }
}


