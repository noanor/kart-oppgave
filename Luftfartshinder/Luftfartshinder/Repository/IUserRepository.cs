using Luftfartshinder.Models;
using Microsoft.AspNetCore.Identity;

namespace Luftfartshinder.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<ApplicationUser>> GetAll();
    }
}
