using BikeRental.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using web;

namespace BikeRental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BikesController : ControllerBase
    {
        private readonly BikeRentalContext _context;

        public BikesController(BikeRentalContext context)
        {
            _context = context;
        }

        // GET: api/Bikes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bike>>> GetBikes([FromQuery] string sort)
        {
            var bikes = await _context.Bikes.Include(b => b.Rentals).Where(b => b.LastService != null).ToArrayAsync();

            switch (sort)
            {
                case "PriceFirstHour": return bikes.OrderBy(b => b.RentalPriceFirstHour).ToArray();
                case "PriceAdditionalHours": return bikes.OrderBy(b => b.RentalPriceAdditionalHours).ToArray();
                case "PurchaseDate": return bikes.OrderBy(b => b.PurchaseDate).ToArray();
                default: return bikes;
            };
        }

        // GET: api/Bikes/5
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Bike>> GetBike(int id)
        {
            var bike = await _context.Bikes.FindAsync(id);

            if (bike == null)
            {
                return NotFound();
            }

            return bike;
        }

        // PUT: api/Bikes/5
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> PutBike(int id, [FromBody] Bike bike)
        {
            if (id != bike.BikeId)
            {
                return BadRequest();
            }

            using var transaction = _context.Database.BeginTransaction();
            _context.Entry(bike).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Bikes.Any(e => e.BikeId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Bikes
        [HttpPost]
        public async Task<ActionResult<Bike>> PostBike([FromBody] Bike bike)
        {
            using var transaction = _context.Database.BeginTransaction();

            _context.Bikes.Add(bike);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Created("Created Bike", bike);
        }

        // DELETE: api/Bikes/5
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<Bike>> DeleteBike(int id)
        {
            using var transaction = _context.Database.BeginTransaction();

            var bike = await _context.Bikes.FindAsync(id);
            if (bike == null)
            {
                return NotFound();
            }

            _context.Bikes.Remove(bike);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return bike;
        }
    }
}
