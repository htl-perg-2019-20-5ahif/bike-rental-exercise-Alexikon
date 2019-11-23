using BikeRental.Model;
using Microsoft.EntityFrameworkCore;

namespace web
{
    public class BikeRentalContext : DbContext
    {
        public DbSet<Bike> Bikes { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public BikeRentalContext(DbContextOptions<BikeRentalContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Rentals)
                .WithOne(r => r.Renter)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.RentedBike)
                .WithMany(b => b.Rentals)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
