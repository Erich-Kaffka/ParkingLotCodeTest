using Microsoft.EntityFrameworkCore;

namespace ParkingLotCodeTest.Models
{
    /// <summary>
    /// Database context for the parking project
    /// </summary>
    public class ParkingContext : DbContext
    {
        public ParkingContext(DbContextOptions<ParkingContext> options) : base(options)
        {

        }

        /// <summary>
        /// Data set containing parking lots
        /// </summary>
        public DbSet<ParkingLot> ParkingLot { get; set; }

        /// <summary>
        /// Data set containing tickets
        /// </summary>
        public DbSet<Ticket> Tickets { get; set; }

        /// <summary>
        /// Data set contianing Payments
        /// </summary>
        public DbSet<Payment> Payments { get; set; }
    }
}
