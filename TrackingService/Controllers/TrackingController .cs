using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using TrackingService.Models;
using System.Threading.Tasks;
using System;
using TrackingService.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TrackingService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TrackingController : ControllerBase
    {
        private readonly TrackingContext _context;

        public TrackingController(TrackingContext context)
        {
            _context = context;
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<Tracking>> GetTracking(int orderId)
        {
            var tracking = await _context.Trackings
                .Where(t => t.OrderId == orderId)
                .OrderByDescending(t => t.UpdatedAt)
                .FirstOrDefaultAsync();

            if (tracking == null)
            {
                return NotFound();
            }

            return tracking;
        }

        [HttpPost]
        public async Task<ActionResult<Tracking>> PostTracking(OrderStatus orderStatus)
        {
            var tracking = new Tracking
            {
                OrderId = orderStatus.OrderId,
                Status = orderStatus.Status,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Trackings.Add(tracking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTracking), new { orderId = tracking.OrderId }, tracking);
        }
    }
}
