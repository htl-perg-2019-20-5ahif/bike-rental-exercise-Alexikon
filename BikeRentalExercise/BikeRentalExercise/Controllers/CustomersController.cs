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
    public class CustomersController : ControllerBase
    {
        private readonly BikeRentalContext _context;

        public CustomersController(BikeRentalContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        // GET: api/Customers/5
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // GET: api/Customers?filter="<filter>"
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomer([FromQuery(Name = "filter")] string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return await _context.Customers.Include(c => c.Rentals).ToArrayAsync();
            }
            return await _context.Customers.Include(c => c.Rentals).Where(c => c.LastName.Contains(filter)).ToArrayAsync();
        }

        // PUT: api/Customers/5
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> PutCustomer(int id, [FromBody] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            using var transaction = _context.Database.BeginTransaction();
            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Customers.Any(e => e.CustomerId == id))
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

        // POST: api/Customers
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer([FromBody] Customer customer)
        {
            using var transaction = _context.Database.BeginTransaction();

            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Created("Created Customer", customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Customer>> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            using var transaction = _context.Database.BeginTransaction();

            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return customer;
        }

        // GET: api/Customers/5
        [HttpGet]
        [Route("{id/rentals}")]
        public async Task<ActionResult<Customer>> GetCustomerRentals(int id)
        {
            var customer = await _context.Customers
                .Where(c => c.CustomerId == id)
                .Include(c => c.Rentals)
                .FirstOrDefaultAsync();

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }
    }
}
