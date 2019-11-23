using BikeRental.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using web;

namespace BikeRental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly BikeRentalContext _context;
        private readonly ICostCalculator _calculator;

        public RentalsController(BikeRentalContext context, ICostCalculator calculator)
        {
            _context = context;
            _calculator = calculator;
        }

        // GET: api/Rentals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rental>>> GetRentals()
        {
            return await _context.Rentals
                .Include(r => r.RentedBike)
                .Include(r => r.Renter)
                .ToArrayAsync();
        }

        // GET: api/Rentals/5
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Rental>> GetRental(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);

            if (rental == null)
            {
                return NotFound();
            }

            return rental;
        }

        // PUT: api/Rentals/5
        [HttpPost]
        public async Task<IActionResult> PutRental([FromBody] Rental rental)
        {
            using var transaction = _context.Database.BeginTransaction();

            _context.Rentals.Add(rental);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Created("Created Rental", rental);
        }

        // PUT: api/Rentals/5
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> PutRental(int id, [FromBody] Rental rental)
        {
            if (id != rental.RentalId)
            {
                return BadRequest();
            }

            _context.Entry(rental).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Rentals.Any(e => e.RentalId == id))
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

        // DELETE: api/Rentals/5
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<Rental>> DeleteRental(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }

            using var transaction = _context.Database.BeginTransaction();

            _context.Rentals.Remove(rental);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return rental;
        }

        // POST: api/Rentals/5/end
        [HttpPost]
        [Route("{id}/end")]
        public async Task<ActionResult<Rental>> EndRental(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);

            if (rental is null)
            {
                return NotFound();
            }

            if (rental.RentalEnd != default)
            {
                return BadRequest();
            }

            using var transaction = _context.Database.BeginTransaction();

            rental.RentalEnd = DateTime.Now;
            rental.TotalRentalCosts = _calculator.Calculate(
                    rental.RentalBegin,
                    rental.RentalEnd.GetValueOrDefault(),
                    rental.RentedBike.RentalPriceFirstHour,
                    rental.RentedBike.RentalPriceAdditionalHours
                );

            _context.Entry(rental).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return rental;
        }

        // POST: api/Rentals/5/paid
        [HttpPost]
        [Route("{id}/paid")]
        public async Task<IActionResult> PayRental(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);

            if (rental == null)
            {
                return NotFound();
            }

            using var transaction = _context.Database.BeginTransaction();

            rental.Paid = true;

            _context.Entry(rental).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return NoContent();
        }

        // GET: api/Rentals
        [HttpGet]
        [Route("unpaid")]
        public async Task<ActionResult<IEnumerable<Rental>>> GetUnpaidRentals()
        {
            return await _context.Rentals
                .Where(r => r.RentalEnd != null && !r.Paid && r.TotalRentalCosts > 0)
                .Include(r => r.RentedBike)
                .Include(r => r.Renter)
                .ToArrayAsync();
        }
    }
}
