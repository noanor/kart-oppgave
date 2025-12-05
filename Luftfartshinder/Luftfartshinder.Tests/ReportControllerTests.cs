using Luftfartshinder.Controllers.Obstacles;
using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.Obstacles;
using Luftfartshinder.Repository;
using Luftfartshinder.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

namespace Luftfartshinder.Tests
{
    public class ReportControllerTests
    {
        private readonly Mock<IObstacleRepository> repositoryMock;
        private readonly TestSession session;
        private readonly ReportController controller;

        public ReportControllerTests()
        {
            repositoryMock = new Mock<IObstacleRepository>();
            session = new TestSession();

            controller = new ReportController(repositoryMock.Object);

            // Setup HttpContext with session
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(h => h.Session).Returns(session);
            
            // Setup TempData
            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempDataDictionary = new TempDataDictionary(httpContextMock.Object, tempDataProvider.Object);
            controller.TempData = tempDataDictionary;
            
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }

        [Fact]
        public async Task Add_NoDraft_ReturnsRedirect()
        {
            // Arrange - Session is empty
            session.Clear();

            // Act
            var result = await controller.Add();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Draft", redirectResult.ActionName);
            Assert.Equal("Obstacles", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Add_EmptyDraft_ReturnsRedirect()
        {
            // Arrange
            var draft = new ObstacleDraftViewModel
            {
                Obstacles = new List<Obstacle>()
            };
            // Use the Set extension method to properly serialize the draft
            session.Set("ObstacleDraft", draft);

            // Act
            var result = await controller.Add();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Draft", redirectResult.ActionName);
            Assert.Equal("Obstacles", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Add_ValidDraft_AddsObstaclesAndRedirects()
        {
            // Arrange
            var obstacle = new Obstacle
            {
                Type = "Mast",
                Name = "Test Obstacle",
                Height = 50,
                Latitude = 59.9139,
                Longitude = 10.7522
            };

            var draft = new ObstacleDraftViewModel
            {
                Obstacles = new List<Obstacle> { obstacle }
            };
            // Use the Set extension method to properly serialize the draft
            session.Set("ObstacleDraft", draft);

            repositoryMock.Setup(r => r.AddObstacle(It.IsAny<Obstacle>())).ReturnsAsync(obstacle);

            // Act
            var result = await controller.Add();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
            repositoryMock.Verify(r => r.AddObstacle(It.IsAny<Obstacle>()), Times.Once);
            Assert.Null(session.Get<ObstacleDraftViewModel>("ObstacleDraft")); // Verify draft was removed
        }
    }
}

