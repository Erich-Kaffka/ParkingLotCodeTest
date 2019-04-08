using System;

namespace ParkingLotCodeTest.Models
{
    /// <summary>
    /// Ticket class represents a model for a parking ticket
    /// </summary>
    public class Ticket
    {
        /// <summary>
        /// Identifier for parking ticket
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Number for the ticket given at check-in and used in check-out
        /// </summary>
        public long TicketNumber { get; set; }

        /// <summary>
        /// Total owing for the parking ticket
        /// </summary>
        public double TotalOwing { get; set; }

        /// <summary>
        /// Time the parking ticket was issued
        /// </summary>
        public DateTime TimeEntered { get; set; }

        /// <summary>
        /// Is the parking ticket paid for
        /// </summary>
        public bool IsPaid { get; set; }
    }
}