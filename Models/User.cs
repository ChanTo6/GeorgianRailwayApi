namespace GeorgianRailwayApi.Models
{
    public enum UserRole
    {
        User,
        Admin
    }

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public bool IsVerified { get; set; } = false;
        public string? VerificationPin { get; set; }
    }
}
