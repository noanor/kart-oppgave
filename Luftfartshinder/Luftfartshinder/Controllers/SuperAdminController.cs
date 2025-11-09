using Luftfartshinder.Models.ViewModel;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Luftfartshinder.Controllers
{

    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private readonly IUserRepository userRepository;


        public SuperAdminController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public async Task<IActionResult> List()
        {
            var users = await userRepository.GetAll();

            var userViewModel = new UserViewModel();
            userViewModel.Users = new List<User>();
            foreach (var user in users)
            {

                userViewModel.Users.Add(new Models.ViewModel.User
                {
                    Id = Guid.Parse(user.Id),
                    UserName = user.UserName,
                    EmailAdress = user.Email
                });

            }
            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete (Guid id)


        {

            return View();
        }
    }

}
