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
        /// MÅL: Teste at hjemmesiden kan lastes uten feil
        /// LOGIKK: Kaller HomeController.Index() og verifiserer at en view returneres
        /// RESULTAT: ViewResult skal returneres uten null-verdier
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


