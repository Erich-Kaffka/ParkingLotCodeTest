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
    /// Web API controller for Ticket CRUD
    /// </summary>
    [Route("api/Tickets")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ParkingContext _context;

        /// <summary>
        /// Initializes the ParkingContext for use
        /// </summary>
        /// <param name="context">ParkingContext</param>
        public TicketController(ParkingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/Tickets - Retrieve all Ticket items
        /// </summary>
        /// <returns>List of Ticket items</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetAllTickets()
        {
            try
            {
                return await _context.Tickets.ToListAsync();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// GET: api/Tickets/{id} - Retrieve Ticket item
        /// </summary>
        /// <param name="id">Ticket Id</param>
        /// <returns>Ticket item</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicket(long id)
        {
            try
            {
                var ticket = await _context.Tickets.FindAsync(id);

                if (ticket == null)
                {
                    return StatusCode((int)HttpStatusCode.NotFound, "Requested ticket not found.");
                }

                //Calculate time difference to return proper owing amount
                TimeSpan timeDifference = ticket.TimeEntered - DateTime.Now;
                double amtOwing = 3;

                if (timeDifference.Hours <= 1) { }
                else if (timeDifference.Hours <= 3) { amtOwing = 4.5; }
                else if (timeDifference.Hours <= 6) { amtOwing = 6; }
                else if (timeDifference.Hours > 6) { amtOwing = 7.5; }

                ticket.TotalOwing = amtOwing;

                return ticket;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// POST: api/Tickets - Create Ticket item
        /// </summary>
        /// <param name="item">Ticket DTO</param>
        /// <returns>Created item</returns>
        [HttpPost]
        public async Task<ActionResult> CreateTicket(Ticket item)
        {
            try
            {
                var lot = await _context.ParkingLot.FirstOrDefaultAsync();

                if (lot.IsFull)
                {
                    return StatusCode((int)HttpStatusCode.MethodNotAllowed, "Parking lot is full!");
                }

                //Increment spots used and update parking lot
                lot.SpotsUsed++;
                if (lot.SpotsUsed == lot.Capacity)
                {
                    lot.IsFull = true;
                }
                _context.Tickets.Add(item);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// PUT: api/Tickets/{id} - Update Ticket item
        /// </summary>
        /// <param name="id">Id of Ticket</param>
        /// <param name="item">Ticket DTO</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(long id, Ticket item)
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
        /// DELETE: api/Ticket/{id} - Deletes specified Ticket
        /// </summary>
        /// <param name="id">Id of Ticket</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(long id)
        {
            try
            {
                var ticket = await _context.Tickets.FindAsync(id);

                if (ticket == null)
                {
                    return StatusCode((int)HttpStatusCode.NotFound, "Requested ticket not found.");
                }

                _context.Tickets.Remove(ticket);
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