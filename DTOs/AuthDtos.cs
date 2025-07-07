namespace GeorgianRailwayApi.DTOs
{
    public class RegisterRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User";
    }

    public class RegisterResponseDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

    public class LoginRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public string Token { get; set; }
    }
}
