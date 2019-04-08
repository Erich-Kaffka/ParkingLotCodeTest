namespace ParkingLotCodeTest.Models
{
    /// <summary>
    /// Payment class represents a model for a parking ticket payment
    /// </summary>
    public class Payment
    {
        /// <summary>
        /// Identifier for parking ticket payment
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Ticket number which the payment is for
        /// </summary>
        public long TicketNumber { get; set; }

        /// <summary>
        /// Credit card number which will pay the ticket
        /// </summary>
        public long CreditCardNumber { get; set; }
    }
}
