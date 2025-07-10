using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeorgianRailwayApi.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        public int TrainId { get; set; }
        [ForeignKey("TrainId")]
        public Train Train { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public int SeatNumber { get; set; }

        public bool IsBooked { get; set; } = false;

        public DateTime? PurchaseDate { get; set; } 
    }
}
