using System;

namespace TrackingService.Models
{
    public class Tracking
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
