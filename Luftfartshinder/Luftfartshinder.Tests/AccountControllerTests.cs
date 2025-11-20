using Luftfartshinder.Controllers;
using Luftfartshinder.DataContext;
using Luftfartshinder.Models;
using Luftfartshinder.Models.ViewModel;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Luftfartshinder.Tests
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> signInManagerMock;
        private readonly AccountController controller;
        private readonly IAccountRepository accountRepository;
        private IReportRepository reportRepository;

        public AccountControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            var contextAccessorMock = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                contextAccessorMock.Object,
                claimsFactoryMock.Object,
                null, null, null, null);

            controller = new AccountController(userManagerMock.Object, signInManagerMock.Object, accountRepository, reportRepository);
        }

        [Fact]
        public void Login_GET_ReturnsView()
        {
            var result = controller.Login();
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task Login_POST_InvalidCredentials_ReturnsViewWithError()
        {
            var model = new LoginViewModel { Username = "testuser", Password = "wrongpassword" };
            
            userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);
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
                IsApproved = false 
            };

            userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            var result = await controller.Login(model);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Contains("pending approval", controller.ModelState[""].Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Login_POST_ValidCredentials_RedirectsToHome()
        {
            var model = new LoginViewModel { Username = "testuser", Password = "password123" };
            var user = new ApplicationUser 
            { 
                UserName = "testuser", 
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
        public async Task Login_POST_SuperAdmin_RedirectsToSuperAdminHome()
        {
            var model = new LoginViewModel { Username = "admin", Password = "password123" };
            var user = new ApplicationUser 
            { 
                UserName = "admin", 
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
            Assert.Equal("SuperAdminHome", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }
    }
}

