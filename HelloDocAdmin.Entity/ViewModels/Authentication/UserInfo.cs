namespace HelloDocAdmin.Entity.ViewModels.Authentication
{
    public class UserInfo
    {

        public int? UserId { get; set; }
        public string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }

        public int? RoleID { get; set; }
        public string? AspUserID { get; set; }
    }
}