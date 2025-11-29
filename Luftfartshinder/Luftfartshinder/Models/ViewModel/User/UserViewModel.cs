using Luftfartshinder.Models.Domain;

namespace Luftfartshinder.Models.ViewModel.User
{
    /// <summary>
    /// ViewModel for displaying a list of users in the SuperAdmin interface.
    /// </summary>
    public class UserViewModel
    {
        /// <summary>List of users to display.</summary>
        public List<Domain.User> Users { get; set; } = [];
    }
}
