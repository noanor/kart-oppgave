/*using Luftfartshinder.Controllers;
using Luftfartshinder.Models;
using Luftfartshinder.Models.ViewModel;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

namespace Luftfartshinder.Tests
{
    public class SuperAdminControllerTests
    {
        private readonly Mock<IUserRepository> userRepositoryMock;
        private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
        private readonly SuperAdminController controller;

        public SuperAdminControllerTests()
        {
            userRepositoryMock = new Mock<IUserRepository>();
            
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            controller = new SuperAdminController(userRepositoryMock.Object, userManagerMock.Object);
            
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;
        }

        [Fact]
        public async Task List_ReturnsViewWithUsers()
        {
            var userId1 = Guid.NewGuid().ToString();
            var userId2 = Guid.NewGuid().ToString();
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = userId1, UserName = "user1", Email = "user1@test.com", IsApproved = true },
                new ApplicationUser { Id = userId2, UserName = "user2", Email = "user2@test.com", IsApproved = false }
            };

            userRepositoryMock.Setup(x => x.GetAll())
                .ReturnsAsync(users);
            userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "FlightCrew" });

            var result = await controller.List();
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserViewModel>(viewResult.Model);
            Assert.Equal(2, model.Users.Count);
        }

        [Fact]
        public async Task List_FiltersByRole_ReturnsOnlyMatchingUsers()
        {
            var userId1 = Guid.NewGuid().ToString();
            var userId2 = Guid.NewGuid().ToString();
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = userId1, UserName = "user1", Email = "user1@test.com", IsApproved = true },
                new ApplicationUser { Id = userId2, UserName = "user2", Email = "user2@test.com", IsApproved = true }
            };

            userRepositoryMock.Setup(x => x.GetAll())
                .ReturnsAsync(users);
            userManagerMock.SetupSequence(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "FlightCrew" })
                .ReturnsAsync(new List<string> { "Registrar" });

            var result = await controller.List("FlightCrew");
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserViewModel>(viewResult.Model);
            Assert.Single(model.Users);
            Assert.Equal("FlightCrew", model.Users.First().Role);
        }

        [Fact]
        public async Task Approve_UserExists_UpdatesIsApproved()
        {
            var userId = Guid.NewGuid();
            var user = new ApplicationUser 
            { 
                Id = userId.ToString(), 
                UserName = "testuser", 
                IsApproved = false 
            };

            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var result = await controller.Approve(userId);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);
            Assert.True(user.IsApproved);
        }

        [Fact]
        public async Task Approve_UserNotFound_RedirectsToList()
        {
            var userId = Guid.NewGuid();

            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);

            var result = await controller.Approve(userId);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);
        }

        [Fact]
        public async Task Delete_UserExists_DeletesUser()
        {
            var userId = Guid.NewGuid();
            var user = new ApplicationUser 
            { 
                Id = userId.ToString(), 
                UserName = "testuser" 
            };

            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var result = await controller.Delete(userId);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);
            userManagerMock.Verify(x => x.DeleteAsync(user), Times.Once);
        }

        [Fact]
        public async Task Decline_UserExists_DeletesUser()
        {
            var userId = Guid.NewGuid();
            var user = new ApplicationUser 
            { 
                Id = userId.ToString(), 
                UserName = "testuser" 
            };

            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var result = await controller.Decline(userId);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectResult.ActionName);
            userManagerMock.Verify(x => x.DeleteAsync(user), Times.Once);
        }
    }
}

*/