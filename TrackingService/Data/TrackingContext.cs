using Microsoft.EntityFrameworkCore;
using TrackingService.Models;

namespace TrackingService.Data
{
    public class TrackingContext : DbContext
    {
        public TrackingContext(DbContextOptions<TrackingContext> options) : base(options)
        {
        }

        public DbSet<Tracking> Trackings { get; set; }
    }
}
