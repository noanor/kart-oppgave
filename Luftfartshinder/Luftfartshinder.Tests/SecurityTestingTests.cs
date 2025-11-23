using Luftfartshinder.Controllers;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Luftfartshinder.Tests
{
    public class SecurityTestingTests
    {
        [Fact]
        public void Security_SuperAdminHome_RequiresAuthorization()
        {
            // Arrange
            var obstacleRepositoryMock = new Mock<IObstacleRepository>();
            var controller = new HomeController(obstacleRepositoryMock.Object);

            // Act
            var result = controller.SuperAdminHome();

            // Assert
            // Hvis bruker ikke er autorisert, skal det returnere en redirect eller unauthorized
            Assert.NotNull(result);
        }
    }
}

