using Luftfartshinder.Controllers;
using Luftfartshinder.Models.Domain;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Luftfartshinder.Tests
{
    public class DashboardControllerTests
    {
        private readonly Mock<IReportRepository> reportRepositoryMock;
        private readonly Mock<IObstacleRepository> obstacleRepositoryMock;
        private readonly Mock<IOrganizationRepository> organizationRepositoryMock;
        private readonly Mock<IAccountRepository> accountRepositoryMock;
        private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
        private readonly DashboardController controller;

        public DashboardControllerTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            reportRepositoryMock = new Mock<IReportRepository>();
            obstacleRepositoryMock = new Mock<IObstacleRepository>();
            organizationRepositoryMock = new Mock<IOrganizationRepository>();
            accountRepositoryMock = new Mock<IAccountRepository>();

            controller = new DashboardController(
                reportRepositoryMock.Object,
                obstacleRepositoryMock.Object,
                organizationRepositoryMock.Object,
                accountRepositoryMock.Object,
                userManagerMock.Object);
        }

        [Fact]
        public async Task Dashboard_ReturnsViewWithReports()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "Registrar")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(h => h.User).Returns(principal);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

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
            var result = await controller.Dashboard();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public async Task Dashboard_ReturnsView()
        {
            // Arrange - Authorize attribute is tested at integration level
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Registrar")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(h => h.User).Returns(principal);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            var reports = new List<Report>();
            reportRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(reports);

            // Act
            var result = await controller.Dashboard();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public async Task FlightCrewObstacles_UserWithOrganization_ReturnsView()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, "pilot"),
                new Claim(ClaimTypes.Role, "FlightCrew")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(h => h.User).Returns(principal);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "pilot",
                OrganizationId = 1
            };

            var organization = new Organization
            {
                Id = 1,
                Name = "Test Organization"
            };

            userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            organizationRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(organization);
            obstacleRepositoryMock.Setup(r => r.GetByOrgId(1)).ReturnsAsync(new List<Obstacle>());
            accountRepositoryMock.Setup(r => r.GetUserReports(userId)).ReturnsAsync(new List<Report>());

            // Act
            var result = await controller.FlightCrewObstacles();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public async Task FlightCrewObstacles_UserNotAuthenticated_ReturnsChallenge()
        {
            // Arrange
            var httpContextMock = new Mock<HttpContext>();
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);
            httpContextMock.Setup(h => h.User).Returns(principal);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await controller.FlightCrewObstacles();

            // Assert
            Assert.IsType<ChallengeResult>(result);
        }

        [Fact]
        public async Task FlightCrewObstacles_UserWithoutOrganization_ReturnsForbid()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, "pilot"),
                new Claim(ClaimTypes.Role, "FlightCrew")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(h => h.User).Returns(principal);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "pilot",
                OrganizationId = 0
            };

            userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            // Act
            var result = await controller.FlightCrewObstacles();

            // Assert
            Assert.IsType<ForbidResult>(result);
        }
    }
}

