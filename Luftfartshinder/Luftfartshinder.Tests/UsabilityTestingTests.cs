using Luftfartshinder.Controllers;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Luftfartshinder.Tests
{
    public class UsabilityTestingTests
    {
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


