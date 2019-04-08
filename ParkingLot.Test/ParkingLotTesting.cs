using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParkingLotCodeTest.Models;
using ParkingLotCodeTest.Controllers;
using Microsoft.EntityFrameworkCore;

namespace ParkingLot.Test
{
    [TestClass]
    public class ParkingLotTesting
    {
        /// <summary>
        /// Testing the usage of the program for the code challenge
        /// </summary>
        [TestMethod]
        public async Task TestFullTicketingSystem()
        {
            //Initialize DbContext
            var optionsBuilder = new DbContextOptionsBuilder<ParkingContext>();
            optionsBuilder.UseInMemoryDatabase();
            var context = new ParkingContext(optionsBuilder.Options);

            //Create controllers for lot testing
            var lotController = new ParkingLotController(context);
            var ticketController = new TicketController(context);
            var paymentController = new PaymentController(context);

            var lotItem = lotController.GetParkingLot(1).Result.Value;

            //Test initialized lot is empty
            Assert.IsFalse(lotItem.IsFull);
            Assert.AreEqual(0, lotItem.SpotsUsed);

            //Create a ticket representing a customer entering the parking lot
            Ticket firstTicket = new Ticket() { TicketNumber = 1, TimeEntered = DateTime.Now };
            await ticketController.CreateTicket(firstTicket);

            //Test the lot is updating as well
            Assert.IsFalse(lotItem.IsFull);
            Assert.AreEqual(1, lotItem.SpotsUsed);

            var updatedTicket = await ticketController.GetTicket(firstTicket.TicketNumber);

            //Test the ticket's total owing is proper value based on short period of time in the lot (up to an hour is 3$)
            Assert.AreEqual(3, updatedTicket.Value.TotalOwing);

            //Complete the transaction by paying for the ticket and exiting the lot
            await paymentController.CreatePayment(new Payment() { TicketNumber = firstTicket.TicketNumber, CreditCardNumber = 1234123412341234 });

            //Check to see the lot is still updated and emptied
            Assert.IsFalse(lotItem.IsFull);
            Assert.AreEqual(0, lotItem.SpotsUsed);
        }

        //ToDo:
        // - Test CRUD on all controllers to ensure it works even though not used
        // - Test expected failures
        //   - Parking lot full
        //   - Wrong ticket number check
        //   - Bad credit card number
    }
}
