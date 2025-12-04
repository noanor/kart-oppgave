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
        /// MÅL: Teste at HomeController.Index() returnerer en view
        /// LOGIKK: Kaller Index() metoden og sjekker returtypen
        /// RESULTAT: ViewResult skal returneres
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


