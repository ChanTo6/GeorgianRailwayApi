namespace GeorgianRailwayApi.DTOs
{
    public class UpdateRequestDto
    {
        public int Id { get; set; }
        public string? Email { get; set; } // Make nullable
        public string? Password { get; set; } // Make nullable
        public string? Role { get; set; } // Add Role as nullable for admin scenarios
    }
}
