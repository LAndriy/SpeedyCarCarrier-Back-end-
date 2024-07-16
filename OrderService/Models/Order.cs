using AuthService.Models;
using System;
using System.Collections.Generic;

namespace OrderService.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int ProdYear { get; set; }
        public string PickupLocation { get; set; }
        public string DeliveryLocation { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        //public string UserId { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}
