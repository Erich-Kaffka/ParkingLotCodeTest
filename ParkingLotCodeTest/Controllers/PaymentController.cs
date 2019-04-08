using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingLotCodeTest.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ParkingLotCodeTest.Controllers
{
    /// <summary>
    /// Web API Controller for Payment CRUD
    /// </summary>
    [Route("api/Payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ParkingContext _context;

        /// <summary>
        /// Initializes the ParkingContext for use
        /// </summary>
        /// <param name="context">ParkingContext</param>
        public PaymentController(ParkingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/Payments - Retrieve all Payment items
        /// </summary>
        /// <returns>List of Payment items</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetAllPayments()
        {
            try
            {
                return await _context.Payments.ToListAsync();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// GET: api/Payments/{id} - Retrieve Payment item
        /// </summary>
        /// <param name="id">Payment Id</param>
        /// <returns>Payment item</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(long id)
        {
            try
            {
                var Payment = await _context.Payments.FindAsync(id);

                if (Payment == null)
                {
                    return StatusCode((int)HttpStatusCode.NotFound, "Requested payment not found.");
                }

                return Payment;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// POST: api/Payments - Create Payment item
        /// </summary>
        /// <param name="item">Payment DTO</param>
        /// <returns>Created item</returns>
        [HttpPost]
        public async Task<ActionResult<Payment>> CreatePayment(Payment item)
        {
            try
            {
                var ticket = await _context.Tickets.FindAsync(item.TicketNumber);

                if (ticket == null)
                {
                    return StatusCode((int)HttpStatusCode.NotFound, "Requested ticket not found.");
                }
                else if (ticket.IsPaid)
                {
                    return StatusCode((int)HttpStatusCode.MethodNotAllowed, "Your ticket is already paid.");
                }

                //Check format of credit card number and return error if "incorrect"
                if (item.CreditCardNumber.ToString().Length == 16 || item.CreditCardNumber.ToString().Length == 20)
                {
                    _context.Payments.Add(item);
                    ticket.IsPaid = true;
                    var lot = await _context.ParkingLot.FirstOrDefaultAsync();
                    lot.SpotsUsed--;

                    //Decrement spots used in the parking lot and pay the ticket
                    await _context.SaveChangesAsync();

                    return CreatedAtAction(nameof(GetPayment), new { id = item.Id }, item);
                }
                else return StatusCode((int)HttpStatusCode.BadRequest, "Credit card format incorrect.");
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// PUT: api/Payments/{id} - Update Payment item
        /// </summary>
        /// <param name="id">Id of Payment</param>
        /// <param name="item">Payment DTO</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(long id, Payment item)
        {
            try
            {
                if (id != item.Id)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, "Parameter mismatch.");
                }

                _context.Entry(item).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// DELETE: api/Payment/{id} - Deletes specified Payment
        /// </summary>
        /// <param name="id">Id of Payment</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(long id)
        {
            try
            {
                var Payment = await _context.Payments.FindAsync(id);

                if (Payment == null)
                {
                    return StatusCode((int)HttpStatusCode.NotFound, "Requested payment not found.");
                }

                _context.Payments.Remove(Payment);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}