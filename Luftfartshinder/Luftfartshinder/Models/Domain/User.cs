namespace Luftfartshinder.Models.Domain
{
    public class User
    {

        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string EmailAdress { get; set; }
        
        public bool IsApproved { get; set; }
        
        public string Role { get; set; }
        
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}
