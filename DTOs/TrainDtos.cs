namespace GeorgianRailwayApi.DTOs
{
    public class TrainRequestDto
    {
        public int TrainId { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public int TotalSeats { get; set; }
    }

    public class TrainResponseDto
    {
        public int Id { get; set; }
        public int TrainId { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public int TotalSeats { get; set; }
        public int BookedSeats { get; set; }
    }
}
