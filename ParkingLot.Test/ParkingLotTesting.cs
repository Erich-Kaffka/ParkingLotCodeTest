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
            DbContextOptionsBuilder<ParkingContext> optionsBuilder = new DbContextOptionsBuilder<ParkingContext>();
            optionsBuilder.UseInMemoryDatabase();
            ParkingContext context = new ParkingContext(optionsBuilder.Options);

            //Create controllers for lot testing
            ParkingLotController lotController = new ParkingLotController(context);
            TicketController ticketController = new TicketController(context);
            PaymentController paymentController = new PaymentController(context);

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

        /// <summary>
        /// Testing that the parking lot will not accept more vehicles than the capacity
        /// </summary>
        [TestMethod]
        public async Task TestParkingLotFull()
        {
            //Initialize DbContext
            DbContextOptionsBuilder<ParkingContext> optionsBuilder = new DbContextOptionsBuilder<ParkingContext>();
            optionsBuilder.UseInMemoryDatabase();
            ParkingContext context = new ParkingContext(optionsBuilder.Options);

            //Create controllers for lot testing
            ParkingLotController lotController = new ParkingLotController(context);
            TicketController ticketController = new TicketController(context);

            var lotItem = lotController.GetParkingLot(1).Result.Value;

            //Test initialized lot is empty
            Assert.IsFalse(lotItem.IsFull);
            Assert.AreEqual(0, lotItem.SpotsUsed);

            //Create a ticket representing a customer entering the parking lot
            await ticketController.CreateTicket(new Ticket() { TicketNumber = 1, TimeEntered = DateTime.Now });
            await ticketController.CreateTicket(new Ticket() { TicketNumber = 2, TimeEntered = DateTime.Now });
            await ticketController.CreateTicket(new Ticket() { TicketNumber = 3, TimeEntered = DateTime.Now });

            //Test the lot is updating as well
            Assert.IsTrue(lotItem.IsFull);
            Assert.AreEqual(3, lotItem.SpotsUsed);

            await ticketController.CreateTicket(new Ticket() { TicketNumber = 4, TimeEntered = DateTime.Now });

            //Test the lot did not accept the 4th ticket
            Assert.IsTrue(lotItem.IsFull);
            Assert.AreEqual(3, lotItem.SpotsUsed);
        }

        /// <summary>
        /// Check retrieval of incorrect ticket number
        /// </summary>
        [TestMethod]
        public async Task TestBadTicketCheck()
        {
            DbContextOptionsBuilder<ParkingContext> optionsBuilder = new DbContextOptionsBuilder<ParkingContext>();
            optionsBuilder.UseInMemoryDatabase();
            ParkingContext context = new ParkingContext(optionsBuilder.Options);

            //Create controllers for lot testing
            ParkingLotController lotController = new ParkingLotController(context);
            TicketController ticketController = new TicketController(context);

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

            //Search for the incorrect ticket value
            var updatedTicket = await ticketController.GetTicket(2);

            //Test that a ticket was not returned
            Assert.AreEqual(null, updatedTicket.Value);
        }

        /// <summary>
        /// Testing all API methods for ParkingLotController
        /// </summary>
        [TestMethod]
        public async Task TestParkingLotCRUD()
        {
            //Initialize DbContext
            DbContextOptionsBuilder<ParkingContext> optionsBuilder = new DbContextOptionsBuilder<ParkingContext>();
            optionsBuilder.UseInMemoryDatabase();
            ParkingContext context = new ParkingContext(optionsBuilder.Options);

            ParkingLotController lotController = new ParkingLotController(context);

            await lotController.CreateParkingLot(new ParkingLotCodeTest.Models.ParkingLot() { Id = 2, Capacity = 1, IsFull = false, Name = "Lot 2", SpotsUsed = 0 });
            var lot = await lotController.GetParkingLot(2);
            lot.Value.Name = "Lot 3";
            await lotController.UpdateParkingLot(2, lot.Value);
            var sameLot = await lotController.GetParkingLot(2);

            //Expected name change
            Assert.AreEqual(sameLot.Value.Name, "Lot 3");
            await lotController.DeleteParkingLot(2);
            var sameLotNull = await lotController.GetParkingLot(2);

            //Should result null if everything works properly
            Assert.AreEqual(null, sameLotNull.Value);
        }

        /// <summary>
        /// Testing all API methods for TicketController
        /// </summary>
        [TestMethod]
        public async Task TestTicketCRUD()
        {
            //Initialize DbContext
            DbContextOptionsBuilder<ParkingContext> optionsBuilder = new DbContextOptionsBuilder<ParkingContext>();
            optionsBuilder.UseInMemoryDatabase();
            ParkingContext context = new ParkingContext(optionsBuilder.Options);

            TicketController ticketController = new TicketController(context);

            await ticketController.CreateTicket(new Ticket() { Id = 1, TicketNumber = 1, TimeEntered = DateTime.Now });
            var ticket = await ticketController.GetTicket(1);
            ticket.Value.TicketNumber = 2;
            await ticketController.UpdateTicket(1, ticket.Value);
            var sameTicket = await ticketController.GetTicket(1);

            //Expected number change
            Assert.AreEqual(sameTicket.Value.TicketNumber, 2);
            await ticketController.DeleteTicket(1);
            var sameTicketNull = await ticketController.GetTicket(1);

            //Should result null if everything works properly
            Assert.AreEqual(null, sameTicketNull.Value);
        }

        /// <summary>
        /// Testing all API methods for PaymentController
        /// </summary>
        [TestMethod]
        public async Task TestPaymentCRUD()
        {
            //Initialize DbContext
            DbContextOptionsBuilder<ParkingContext> optionsBuilder = new DbContextOptionsBuilder<ParkingContext>();
            optionsBuilder.UseInMemoryDatabase();
            ParkingContext context = new ParkingContext(optionsBuilder.Options);

            PaymentController paymentController = new PaymentController(context);

            await paymentController.CreatePayment(new Payment() { Id = 1, TicketNumber = 1, CreditCardNumber = 1234123412341234 });
            var payment = await paymentController.GetPayment(1);
            payment.Value.TicketNumber = 2;
            await paymentController.UpdatePayment(1, payment.Value);
            var samePayment = await paymentController.GetPayment(1);

            //Expected number change
            Assert.AreEqual(samePayment.Value.TicketNumber, 2);
            await paymentController.DeletePayment(1);
            var samePaymentNull = await paymentController.GetPayment(1);

            //Should result null if everything works properly
            Assert.AreEqual(null, samePaymentNull.Value);
        }
    }
}