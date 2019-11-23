using BikeRental.Model;
using System;

namespace BikeRental
{
    public interface ICostCalculator
    {
        public decimal Calculate(DateTime start, DateTime end, decimal prizeFirstHour, decimal prizeAdditionalHours);
    }
}