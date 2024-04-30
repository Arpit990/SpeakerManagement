using Microsoft.AspNetCore.Identity;

namespace SpeakerManagement.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int OrganizationId { get; set; }
        public string? Website {  get; set; }
    }
}
