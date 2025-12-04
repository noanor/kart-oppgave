using Luftfartshinder.Controllers.Obstacles;
using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.Obstacles;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using Xunit;

namespace Luftfartshinder.Tests
{
    public class ObstaclesControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
        private readonly Mock<IReportRepository> reportRepositoryMock;
        private readonly Mock<IObstacleRepository> obstacleRepositoryMock;
        private readonly TestSession session;
        private readonly ObstaclesController controller;

        public ObstaclesControllerTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            reportRepositoryMock = new Mock<IReportRepository>();
            obstacleRepositoryMock = new Mock<IObstacleRepository>();
            session = new TestSession();

            controller = new ObstaclesController(
                userManagerMock.Object,
                reportRepositoryMock.Object,
                obstacleRepositoryMock.Object);

            // Setup HttpContext with session and TempData
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
        public void Draft_NoDraftInSession_ReturnsViewWithEmptyDraft()
        {
            // Arrange - Session is empty (no draft exists)
            session.Clear();

            // Act
            var result = controller.Draft();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Draft", viewResult.ViewName);
            Assert.Equal("ipad", controller.ViewData["LayoutType"]);
            var model = Assert.IsType<ObstacleDraftViewModel>(viewResult.Model);
            Assert.Empty(model.Obstacles);
        }

        [Fact]
        public void Draft_WithDraftInSession_ReturnsViewWithDraft()
        {
            // Arrange
            var draft = new ObstacleDraftViewModel
            {
                Obstacles = new List<Obstacle>
                {
                    new Obstacle
                    {
                        Type = "Mast",
                        Name = "Test Obstacle",
                        Height = 50,
                        Latitude = 59.9139,
                        Longitude = 10.7522
                    }
                }
            };
            var json = JsonSerializer.Serialize(draft, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            session.SetString("ObstacleDraft", json);

            // Act
            var result = controller.Draft();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ObstacleDraftViewModel>(viewResult.Model);
            Assert.Single(model.Obstacles);
        }

        [Fact]
        public void AddOne_ValidData_AddsObstacleToDraft()
        {
            // Arrange
            session.Clear();
            var request = new ObstaclesController.AddObstacleRequest
            {
                Type = "Mast",
                Name = "New Obstacle",
                Height = 30,
                Latitude = 59.9139,
                Longitude = 10.7522
            };

            // Act
            var result = controller.AddOne(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ObstaclesController.AddOneResponse>(okResult.Value);
            Assert.True(response.Ok);
            Assert.Equal(1, response.Count);
            // Verify that draft was saved to session
            var savedDraft = session.GetString("ObstacleDraft");
            Assert.NotNull(savedDraft);
        }

        [Fact]
        public void AddOne_NullData_ReturnsBadRequest()
        {
            // Act
            var result = controller.AddOne(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ClearDraft_RemovesDraftFromSession()
        {
            // Arrange
            session.SetString("ObstacleDraft", "test");

            // Act
            var result = controller.ClearDraft();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Draft", redirectResult.ActionName);
            Assert.Null(session.GetString("ObstacleDraft"));
        }

        [Fact]
        public async Task SubmitDraft_UserNotAuthenticated_ReturnsChallenge()
        {
            // Arrange
            var httpContextMock = new Mock<HttpContext>();
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);
            httpContextMock.Setup(h => h.User).Returns(principal);
            httpContextMock.Setup(h => h.Session).Returns(session);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            var draft = new ObstacleDraftViewModel
            {
                Obstacles = new List<Obstacle>
                {
                    new Obstacle { Type = "Mast", Name = "Test" }
                }
            };
            var json = JsonSerializer.Serialize(draft, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            session.SetString("ObstacleDraft", json);

            // Act
            var result = await controller.SubmitDraft();

            // Assert
            Assert.IsType<ChallengeResult>(result);
        }

        [Fact]
        public void DraftJson_NoDraft_ReturnsEmptyList()
        {
            // Arrange - Session is empty
            session.Clear();

            // Act
            var result = controller.DraftJson();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            // The controller returns a list of anonymous types, so we check it's enumerable and empty
            var enumerable = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Empty(enumerable);
        }

        [Fact]
        public void DraftJson_WithDraft_ReturnsObstacleList()
        {
            // Arrange
            var draft = new ObstacleDraftViewModel
            {
                Obstacles = new List<Obstacle>
                {
                    new Obstacle
                    {
                        Type = "Mast",
                        Name = "Test",
                        Height = 50,
                        Latitude = 59.9139,
                        Longitude = 10.7522
                    }
                }
            };
            var json = JsonSerializer.Serialize(draft, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            session.SetString("ObstacleDraft", json);

            // Act
            var result = controller.DraftJson();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            // The controller returns a list of anonymous types, so we check it's enumerable
            var enumerable = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            var list = enumerable.ToList();
            Assert.Single(list);
        }
    }
}

