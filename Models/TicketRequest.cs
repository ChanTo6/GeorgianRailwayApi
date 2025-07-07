namespace GeorgianRailwayApi.Models
{
    public class TicketRequest
    {
        public int UserId { get; set; }
        public List<TicketPurchase> Tickets { get; set; }
    }

}
