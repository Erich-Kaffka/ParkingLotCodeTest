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

            //Create controller for lot
            var lotController = new ParkingLotController(context);

            var lotItem = lotController.GetParkingLot(1).Result.Value;

            //Test initialized lot is empty
            Assert.IsFalse(lotItem.IsFull);
            Assert.AreEqual(0, lotItem.SpotsUsed);
        }
    }
}
