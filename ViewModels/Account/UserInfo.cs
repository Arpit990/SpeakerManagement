namespace SpeakerManagement.ViewModels.Account
{
    public class UserInfo
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string OrganizationName {  get; set; }
        public int OrganizationId {  get; set; }
        public string Website {  get; set; }
        public string Role { get; set; }    
        public bool IsActive { get; set; }
    }
}
