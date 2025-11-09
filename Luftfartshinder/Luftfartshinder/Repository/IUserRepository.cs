using Microsoft.AspNetCore.Identity;

namespace Luftfartshinder.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<IdentityUser>> GetAll();
    }
}
