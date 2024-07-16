using OrderService.Models;
using System.Collections.Generic;

namespace TrackingService.Models
{
    public class OrderStatus
    {
        //public virtual ICollection<Order> id { get; set; }
        public int OrderId { get; set; }
        public string Status { get; set; }
    }
}
