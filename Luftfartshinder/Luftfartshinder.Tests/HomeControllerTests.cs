using Luftfartshinder.Controllers;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Luftfartshinder.Tests
{
    public class HomeControllerTests
    {
        private readonly Mock<IObstacleRepository> obstacleRepositoryMock;
        private readonly HomeController controller;

        public HomeControllerTests()
        {
            obstacleRepositoryMock = new Mock<IObstacleRepository>();
            controller = new HomeController(obstacleRepositoryMock.Object);
        }

        [Fact]
        public void Index_ReturnsView()
        {
            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
            Assert.Equal("ipad", controller.ViewData["LayoutType"]);
            Assert.Equal("page-home", controller.ViewData["BodyClass"]);
        }

        [Fact]
        public void Privacy_ReturnsView()
        {
            // Act
            var result = controller.Privacy();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
            Assert.Equal("pc", controller.ViewData["LayoutType"]);
        }

        [Fact]
        public void DataForm_ReturnsView()
        {
            // Act
            var result = controller.DataForm();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
            Assert.Equal("ipad", controller.ViewData["LayoutType"]);
        }

        [Fact]
        public void Error_ReturnsViewWithErrorViewModel()
        {
            // Arrange
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(h => h.TraceIdentifier).Returns("test-trace-id");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            // Act
            var result = controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
            Assert.Equal("pc", controller.ViewData["LayoutType"]);
            var model = Assert.IsType<Luftfartshinder.Models.ViewModel.Shared.ErrorViewModel>(viewResult.Model);
            Assert.NotNull(model.RequestId);
        }

        [Fact]
        public void SuperAdminHome_ReturnsView()
        {
            // Arrange - Authorize attribute is tested at integration level, not unit level
            // For unit tests, we test that the action returns a view when authorized
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "SuperAdmin")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(h => h.User).Returns(principal);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            // Act
            var result = controller.SuperAdminHome();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("pc", controller.ViewData["LayoutType"]);
        }

        [Fact]
        public void RegistrarHome_ReturnsView()
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

            // Act
            var result = controller.RegistrarHome();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("pc", controller.ViewData["LayoutType"]);
        }

        [Fact]
        public void IndexHome_ReturnsView()
        {
            // Arrange - Authorize attribute is tested at integration level
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
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

            // Act
            var result = controller.IndexHome();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.Equal("ipad", controller.ViewData["LayoutType"]);
        }

        [Fact]
        public void Tutorial_ReturnsView()
        {
            // Arrange - Authorize attribute is tested at integration level
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(h => h.User).Returns(principal);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            // Act
            var result = controller.Tutorial();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("ipad", controller.ViewData["LayoutType"]);
        }
    }
}

