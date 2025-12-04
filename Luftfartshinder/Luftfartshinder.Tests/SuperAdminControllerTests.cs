using Luftfartshinder.Controllers.Admin;
using Luftfartshinder.Models.Domain;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Luftfartshinder.Tests
{
    public class SuperAdminControllerTests
    {
        private readonly Mock<IUserRepository> userRepositoryMock;
        private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
        private readonly Mock<IOrganizationRepository> organizationRepositoryMock;
        private readonly SuperAdminController controller;

        public SuperAdminControllerTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            userRepositoryMock = new Mock<IUserRepository>();
            organizationRepositoryMock = new Mock<IOrganizationRepository>();

            controller = new SuperAdminController(
                userRepositoryMock.Object,
                userManagerMock.Object,
                organizationRepositoryMock.Object);

            // Setup authenticated user with SuperAdmin role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, "SuperAdmin")
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
        public async Task List_ReturnsViewWithUsers()
        {
            // Arrange
            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "testuser",
                    Email = "test@test.com",
                    IsApproved = true
                }
            };

            userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(users);
            userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "FlightCrew" });

            // Act
            var result = await controller.List();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("pc", controller.ViewData["LayoutType"]);
        }

        [Fact]
        public async Task List_WithRoleFilter_FiltersUsers()
        {
            // Arrange
            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "pilot1",
                    Email = "pilot1@test.com",
                    IsApproved = true
                },
                new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "registrar1",
                    Email = "registrar1@test.com",
                    IsApproved = true
                }
            };

            userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(users);
            userManagerMock.Setup(m => m.GetRolesAsync(It.Is<ApplicationUser>(u => u.UserName == "pilot1")))
                .ReturnsAsync(new List<string> { "FlightCrew" });
            userManagerMock.Setup(m => m.GetRolesAsync(It.Is<ApplicationUser>(u => u.UserName == "registrar1")))
                .ReturnsAsync(new List<string> { "Registrar" });

            // Act
            var result = await controller.List(roleFilter: "FlightCrew");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Luftfartshinder.Models.ViewModel.User.UserViewModel>(viewResult.Model);
            Assert.Single(model.Users);
        }

        [Fact]
        public async Task Approve_UserExists_UpdatesUserAndRedirects()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser
            {
                Id = userId.ToString(),
                UserName = "testuser",
                IsApproved = false
            };

            userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await controller.Approve(userId, "All", "", "");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);
            Assert.True(user.IsApproved);
        }

        [Fact]
        public async Task Approve_UserNotFound_Redirects()
        {
            // Arrange
            var userId = Guid.NewGuid();
            userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await controller.Approve(userId, "All", "", "");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);
        }

        [Fact]
        public async Task Delete_UserExists_DeletesUserAndRedirects()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser
            {
                Id = userId.ToString(),
                UserName = "testuser"
            };

            userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            userManagerMock.Setup(m => m.DeleteAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await controller.Delete(userId, "All", "", "");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);
        }

        [Fact]
        public async Task Delete_UserNotFound_Redirects()
        {
            // Arrange
            var userId = Guid.NewGuid();
            userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await controller.Delete(userId, "All", "", "");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);
        }

        [Fact]
        public async Task Decline_UserExists_DeletesUserAndRedirects()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser
            {
                Id = userId.ToString(),
                UserName = "testuser"
            };

            userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            userManagerMock.Setup(m => m.DeleteAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await controller.Decline(userId, "All", "", "");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);
        }
    }
}

