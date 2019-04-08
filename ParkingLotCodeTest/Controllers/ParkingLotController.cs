using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingLotCodeTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ParkingLotCodeTest.Controllers
{
    /// <summary>
    /// Web API Controller for Parking lot CRUD
    /// </summary>
    [Route("api/ParkingLot")]
    [ApiController]
    public class ParkingLotController : ControllerBase
    {
        private readonly ParkingContext _context;

        /// <summary>
        /// Initializes the ParkingContext
        /// </summary>
        /// <param name="context">ParkingContext</param>
        public ParkingLotController(ParkingContext context)
        {
            _context = context;

            if (_context.ParkingLot.Count() == 0)
            {
                //Creates first lot which will be used for this project
                _context.ParkingLot.Add(new ParkingLot { Id = 1, Name = "Lot 1", SpotsUsed = 0, Capacity = 3, IsFull = false });
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// GET: api/ParkingLot - Retrieve all ParkingLot items
        /// </summary>
        /// <returns>List of ParkingLot items</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParkingLot>>> GetAllParkingLots()
        {
            try
            {
                return await _context.ParkingLot.ToListAsync();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// GET: api/ParkingLot/{id} - Retrieve ParkingLot item
        /// </summary>
        /// <param name="id">ParkingLot Id</param>
        /// <returns>ParkingLot item</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ParkingLot>> GetParkingLot(long id)
        {
            try
            {
                var parkingLotItem = await _context.ParkingLot.FindAsync(id);

                if (parkingLotItem == null)
                {
                    return StatusCode((int)HttpStatusCode.NotFound, "Requested lot not found.");
                }

                return parkingLotItem;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// POST: api/ParkingLot - Create ParkingLot item
        /// </summary>
        /// <param name="item">ParkingLot DTO</param>
        /// <returns>Created item</returns>
        [HttpPost]
        public async Task<ActionResult<ParkingLot>> CreateParkingLot(ParkingLot item)
        {
            try
            {
                _context.ParkingLot.Add(item);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetParkingLot), new { id = item.Id }, item);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// PUT: api/ParkingLot/{id} - Update ParkingLot item
        /// </summary>
        /// <param name="id">Id of ParkingLot</param>
        /// <param name="item">ParkingLot DTO</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParkingLot(long id, ParkingLot item)
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
        /// DELETE: api/ParkingLot/{id} - Deletes specified ParkingLotItem
        /// </summary>
        /// <param name="id">Id of ParkingLot</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParkingLot(long id)
        {
            try
            {
                var parkingLotItem = await _context.ParkingLot.FindAsync(id);

                if (parkingLotItem == null)
                {
                    return StatusCode((int)HttpStatusCode.NotFound, "Requested lot not found.");
                }

                _context.ParkingLot.Remove(parkingLotItem);
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