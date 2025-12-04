using Luftfartshinder.Controllers.Admin;
using Luftfartshinder.Models.Domain;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Luftfartshinder.Tests
{
    public class RegistrarControllerTests
    {
        private readonly Mock<IReportRepository> reportRepositoryMock;
        private readonly Mock<IObstacleRepository> obstacleRepositoryMock;
        private readonly RegistrarController controller;

        public RegistrarControllerTests()
        {
            reportRepositoryMock = new Mock<IReportRepository>();
            obstacleRepositoryMock = new Mock<IObstacleRepository>();

            controller = new RegistrarController(
                reportRepositoryMock.Object,
                obstacleRepositoryMock.Object);

            // Setup authenticated user with Registrar role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, "registrar"),
                new Claim(ClaimTypes.Role, "Registrar")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(h => h.User).Returns(principal);
            
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
        public async Task Index_ReturnsViewWithReports()
        {
            // Arrange
            var reports = new List<Report>
            {
                new Report
                {
                    Id = 1,
                    Author = "Test User",
                    AuthorId = Guid.NewGuid().ToString(),
                    ReportDate = DateTime.Now,
                    Title = "Test Report"
                }
            };

            reportRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(reports);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.Equal("pc", controller.ViewData["LayoutType"]);
        }

        [Fact]
        public async Task Overview_ReturnsViewWithReports()
        {
            // Arrange
            var reports = new List<Report>
            {
                new Report
                {
                    Id = 1,
                    Author = "Test User",
                    AuthorId = Guid.NewGuid().ToString(),
                    ReportDate = DateTime.Now,
                    Title = "Test Report"
                }
            };

            reportRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(reports);

            // Act
            var result = await controller.Overview();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Overview", viewResult.ViewName);
            Assert.Equal("pc", controller.ViewData["LayoutType"]);
        }

        [Fact]
        public async Task Details_ReportExists_ReturnsView()
        {
            // Arrange
            var report = new Report
            {
                Id = 1,
                Author = "Test User",
                AuthorId = Guid.NewGuid().ToString(),
                ReportDate = DateTime.Now,
                Title = "Test Report",
                Obstacles = new List<Obstacle>()
            };

            reportRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(report);

            // Act
            var result = await controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
            Assert.Equal("pc", controller.ViewData["LayoutType"]);
        }

        [Fact]
        public async Task Details_ReportNotFound_ReturnsNotFound()
        {
            // Arrange
            reportRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Report?)null);

            // Act
            var result = await controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SaveNote_ObstacleExists_UpdatesAndRedirects()
        {
            // Arrange
            var obstacle = new Obstacle
            {
                Id = 1,
                ReportId = 10,
                RegistrarNote = "Old note"
            };

            obstacleRepositoryMock.Setup(r => r.GetObstacleById(1)).ReturnsAsync(obstacle);
            obstacleRepositoryMock.Setup(r => r.UpdateObstacle(It.IsAny<Obstacle>())).ReturnsAsync(obstacle);

            var obstacleData = new Obstacle
            {
                Id = 1,
                ReportId = 10,
                RegistrarNote = "New note"
            };

            // Act
            var result = await controller.SaveNote(obstacleData);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("New note", obstacle.RegistrarNote);
        }

        [Fact]
        public async Task Delete_ReportExists_DeletesAndRedirects()
        {
            // Arrange
            var report = new Report
            {
                Id = 1,
                Author = "Test User",
                AuthorId = Guid.NewGuid().ToString(),
                ReportDate = DateTime.Now,
                Title = "Test Report"
            };

            reportRepositoryMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(report);

            // Act
            var result = await controller.Delete(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Overview", redirectResult.ActionName);
        }

        [Fact]
        public async Task Approve_ObstacleExists_UpdatesStatusAndRedirects()
        {
            // Arrange
            var obstacle = new Obstacle
            {
                Id = 1,
                ReportId = 10,
                Status = Obstacle.Statuses.Pending
            };

            obstacleRepositoryMock.Setup(r => r.GetObstacleById(1)).ReturnsAsync(obstacle);
            obstacleRepositoryMock.Setup(r => r.UpdateObstacle(It.IsAny<Obstacle>())).ReturnsAsync(obstacle);

            // Act
            var result = await controller.Approve(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal(Obstacle.Statuses.Approved, obstacle.Status);
        }

        [Fact]
        public async Task Approve_ObstacleNotFound_ReturnsNotFound()
        {
            // Arrange
            obstacleRepositoryMock.Setup(r => r.GetObstacleById(999)).ReturnsAsync((Obstacle?)null);

            // Act
            var result = await controller.Approve(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Reject_ObstacleExists_UpdatesStatusAndRedirects()
        {
            // Arrange
            var obstacle = new Obstacle
            {
                Id = 1,
                ReportId = 10,
                Status = Obstacle.Statuses.Pending
            };

            obstacleRepositoryMock.Setup(r => r.GetObstacleById(1)).ReturnsAsync(obstacle);
            obstacleRepositoryMock.Setup(r => r.UpdateObstacle(It.IsAny<Obstacle>())).ReturnsAsync(obstacle);

            // Act
            var result = await controller.Reject(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal(Obstacle.Statuses.Rejected, obstacle.Status);
        }

        [Fact]
        public async Task Reject_ObstacleNotFound_ReturnsNotFound()
        {
            // Arrange
            obstacleRepositoryMock.Setup(r => r.GetObstacleById(999)).ReturnsAsync((Obstacle?)null);

            // Act
            var result = await controller.Reject(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}

