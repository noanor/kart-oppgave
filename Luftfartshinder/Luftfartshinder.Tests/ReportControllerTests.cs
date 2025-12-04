using Luftfartshinder.Controllers.Obstacles;
using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.Obstacles;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text.Json;
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
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }

        [Fact]
        public async Task Add_NoDraft_ReturnsBadRequest()
        {
            // Arrange - Session is empty
            session.Clear();

            // Act
            var result = await controller.Add();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Add_EmptyDraft_ReturnsBadRequest()
        {
            // Arrange
            var draft = new ObstacleDraftViewModel
            {
                Obstacles = new List<Obstacle>()
            };
            var json = JsonSerializer.Serialize(draft, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            session.SetString("ObstacleDraft", json);

            // Act
            var result = await controller.Add();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
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
            var json = JsonSerializer.Serialize(draft, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            session.SetString("ObstacleDraft", json);

            repositoryMock.Setup(r => r.AddObstacle(It.IsAny<Obstacle>())).ReturnsAsync(obstacle);

            // Act
            var result = await controller.Add();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
            repositoryMock.Verify(r => r.AddObstacle(It.IsAny<Obstacle>()), Times.Once);
            Assert.Null(session.GetString("ObstacleDraft")); // Verify draft was removed
        }
    }
}

