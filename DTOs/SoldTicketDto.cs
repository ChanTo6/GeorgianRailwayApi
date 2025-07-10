namespace GeorgianRailwayApi.DTOs
{
    public class SoldTicketDto
    {
        public int TicketId { get; set; }
        public string TrainName { get; set; }
        public int TrainId { get; set; }
        public int SeatNumber { get; set; }
        public string BuyerEmail { get; set; }
        public int BuyerId { get; set; }
        public DateTime? PurchaseDate { get; set; } 
    }
}
