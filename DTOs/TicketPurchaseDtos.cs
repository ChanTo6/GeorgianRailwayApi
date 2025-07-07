namespace GeorgianRailwayApi.DTOs
{
    public class TicketPurchaseRequestDto
    {
        public int UserId { get; set; }
        public List<TicketPurchaseDto> Tickets { get; set; }
    }
    public class TicketPurchaseDto
    {
        public int TrainId { get; set; }
        public int SeatNumber { get; set; }
    }
    public class TicketPurchaseResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
