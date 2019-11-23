using System;
using System.ComponentModel.DataAnnotations;

namespace BikeRental.Model
{
    public class Rental
    {
        public int RentalId { get; set; }

        [Required]
        public DateTime RentalBegin { get; set; }

        private DateTime? _rentalEnd { get; set; }
        public DateTime? RentalEnd
        {
            get { return _rentalEnd; }
            set
            {
                if (RentalBegin > value)
                {
                    throw new ArgumentException("RentalEnd must be greater than Begin");
                }

                _rentalEnd = value;
            }
        }

        [Range(0, double.PositiveInfinity)]
        [RegularExpression("^\\d+(\\.\\d{1,2})$")]
        public decimal? TotalRentalCosts { get; set; }

        [Required]
        public bool Paid { get; set; }


        // References
        [Required]
        public int CustomerID { get; set; }

        public Customer Renter { get; set; }

        [Required]
        public int BikeID { get; set; }

        public Bike RentedBike { get; set; }
    }
}
