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
        /// MÅL: Teste at Privacy-siden er tilgjengelig og returnerer en view
        /// LOGIKK: Kaller Privacy() og verifiserer at en view returneres
        /// RESULTAT: ViewResult skal returneres, noe som indikerer at siden er tilgjengelig for brukere
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


