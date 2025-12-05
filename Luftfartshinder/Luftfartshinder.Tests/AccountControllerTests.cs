using Luftfartshinder.Controllers.Account;
using Luftfartshinder.Models.Domain;
using Luftfartshinder.Models.ViewModel.Shared;
using Luftfartshinder.Repository;
using Luftfartshinder.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Luftfartshinder.Tests
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> signInManagerMock;
        private readonly Mock<IAccountRepository> accountRepositoryMock;
        private readonly Mock<IReportRepository> reportRepositoryMock;
        private readonly Mock<IOrganizationRepository> organizationRepositoryMock;
        private readonly Mock<IObstacleRepository> obstacleRepositoryMock;
        private readonly Mock<IOrganizationService> organizationServiceMock;
        private readonly Mock<IUserService> userServiceMock;
        private readonly AccountController controller;

        public AccountControllerTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var contextAccessorMock = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                contextAccessorMock.Object,
                claimsFactoryMock.Object,
                null!, null!, null!, null!);

            accountRepositoryMock = new Mock<IAccountRepository>();
            reportRepositoryMock = new Mock<IReportRepository>();
            organizationRepositoryMock = new Mock<IOrganizationRepository>();
            obstacleRepositoryMock = new Mock<IObstacleRepository>();
            organizationServiceMock = new Mock<IOrganizationService>();
            userServiceMock = new Mock<IUserService>();

            controller = new AccountController(
                userManagerMock.Object,
                signInManagerMock.Object,
                accountRepositoryMock.Object,
                reportRepositoryMock.Object,
                organizationRepositoryMock.Object,
                obstacleRepositoryMock.Object,
                organizationServiceMock.Object,
                userServiceMock.Object);
        }

        /// <summary>
        /// GOAL: Test that Login GET action returns a view
        /// LOGIC: Calls Login() without parameters and checks the return type
        /// RESULT: Method should return a ViewResult
        /// </summary>
        [Fact]
        public void Login_GET_ReturnsView()
        {
            // Arrange - Mock HttpContext and User.Identity
            var httpContextMock = new Mock<HttpContext>();
            var identity = new ClaimsIdentity(); // Not authenticated
            var principal = new ClaimsPrincipal(identity);
            
            httpContextMock.Setup(h => h.User).Returns(principal);
            
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            // Act
            var result = controller.Login();
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        /// <summary>
        /// GOAL: Test that Login POST handles invalid credentials correctly
        /// LOGIC: Attempts to log in with wrong password and verifies that error message is returned
        /// RESULT: ViewResult should be returned with ModelState.IsValid = false
        /// </summary>
        [Fact]
        public async Task Login_POST_InvalidCredentials_ReturnsViewWithError()
        {
            var model = new LoginViewModel { Username = "testuser", Password = "wrongpassword" };
            
            userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);
            signInManagerMock.Setup(x => x.PasswordSignInAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var result = await controller.Login(model);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Login_POST_UserNotApproved_ReturnsViewWithError()
        {
            var model = new LoginViewModel { Username = "testuser", Password = "password123" };
            var user = new ApplicationUser 
            { 
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                IsApproved = false 
            };

            userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            var result = await controller.Login(model);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Contains("pending approval", controller.ModelState[""]!.Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Login_POST_ValidCredentials_RedirectsToHome()
        {
            var model = new LoginViewModel { Username = "testuser", Password = "password123" };
            var user = new ApplicationUser 
            { 
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                IsApproved = true 
            };

            userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            signInManagerMock.Setup(x => x.PasswordSignInAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "FlightCrew" });

            var result = await controller.Login(model);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Login_POST_SuperAdmin_RedirectsToDashboard()
        {
            var model = new LoginViewModel { Username = "admin", Password = "password123" };
            var user = new ApplicationUser 
            { 
                UserName = "admin",
                FirstName = "Admin",
                LastName = "User",
                IsApproved = true 
            };

            userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            signInManagerMock.Setup(x => x.PasswordSignInAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "SuperAdmin" });

            var result = await controller.Login(model);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirectResult.ActionName);
            // ControllerName can be "Dashboard" or null depending on routing
            Assert.True(redirectResult.ControllerName == "Dashboard" || redirectResult.ControllerName == null);
        }
    }
}
