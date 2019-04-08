namespace ParkingLotCodeTest.Models
{
    /// <summary>
    /// ParkingLot class represents a model for a parking lot
    /// </summary>
    public class ParkingLot
    {
        /// <summary>
        /// Identifier for the parking lot
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Name of parking lot
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Number of parking spots unavailable
        /// </summary>
        public int SpotsUsed { get; set; }

        /// <summary>
        /// Maximum number of parking spots available
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// Whether or not the parking lot is at full capacity
        /// </summary>
        public bool IsFull { get; set; }
    }
}