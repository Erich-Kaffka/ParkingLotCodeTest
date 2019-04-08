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
        /// Database to contain set of parking lots
        /// </summary>
        public DbSet<ParkingLot> ParkingLot { get; set; }
    }
}
